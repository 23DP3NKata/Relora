using Relora.Items.Application.Commands;
using Relora.Items.Application.Interfaces;
using Relora.Items.Application.Services;
using Relora.Items.Domain;
using Relora.Items.Domain.Enums;
using Relora.Shared.Domain.Persistence;
using Relora.Shared.Domain.Time;
using Relora.Shared.Domain.ValueObjects;

using MediatR;

namespace Relora.Items.Application.Handlers.Commands;

/// <summary>
/// Represents the create lot command handler class.
/// </summary>
public sealed class CreateLotCommandHandler : IRequestHandler<CreateLotCommand, Guid>
{
    private readonly ILotRepository _lotRepository;
    private readonly ILotCatalogRepository _catalogRepository;
    private readonly IPendingLotMediaUploadRepository _pendingUploads;
    private readonly IPendingLotProofDocumentUploadRepository _pendingProofUploads;
    private readonly ITransactionRunner _transactions;
    private readonly IClock _clock;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateLotCommandHandler"/> class.
    /// </summary>
    public CreateLotCommandHandler(
        ILotRepository lotRepository,
        ILotCatalogRepository catalogRepository,
        IPendingLotMediaUploadRepository pendingUploads,
        IPendingLotProofDocumentUploadRepository pendingProofUploads,
        ITransactionRunner transactions,
        IClock clock)
    {
        _lotRepository = lotRepository;
        _catalogRepository = catalogRepository;
        _pendingUploads = pendingUploads;
        _pendingProofUploads = pendingProofUploads;
        _transactions = transactions;
        _clock = clock;
    }

    /// <summary>
    /// Handles the operation.
    /// </summary>
    public async Task<Guid> Handle(CreateLotCommand request, CancellationToken cancellationToken)
    {
        var money = new Money(request.Amount, request.Currency);
        var catalogInput = await LotCatalogInputResolver.ResolveAsync(
            _catalogRepository,
            request.CategoryId,
            request.Department,
            request.Category,
            request.Gender,
            request.PrimaryColorId,
            request.Color,
            request.Materials,
            request.Measurements,
            cancellationToken);

        var lot = new Lot(
            Guid.NewGuid(),
            request.SellerId,
            request.Title,
            request.Description,
            money,
            catalogInput.CategoryId,
            catalogInput.Department,
            catalogInput.LegacyCategory,
            catalogInput.LegacyGender,
            request.Size,
            request.Brand,
            request.Condition,
            catalogInput.PrimaryColorId,
            request.Color,
            request.Country ?? request.ShippingOriginCountry,
            request.City,
            request.ModelName,
            request.AcquisitionYear,
            request.ProductionYear,
            request.IsVintage || request.Category == LotCategory.Vintage,
            request.VintageNotes,
            request.Age,
            request.Style,
            request.ShippingPrice,
            request.ShippingCurrency,
            request.ShippingOriginCountry,
            request.ShipsToCountries,
            request.ShippingHandlingDays,
            catalogInput.Materials,
            [],
            catalogInput.Measurements,
            _clock.UtcNow);

        var photoKeys = NormalizePhotoKeys(request.PhotoKeys);
        var coverPhotoKey = NormalizeCoverPhotoKey(request.CoverPhotoKey, photoKeys);
        var uploads = await _pendingUploads.GetByOwnerAndKeysAsync(
            request.SellerId,
            photoKeys,
            cancellationToken);

        if (uploads.Count != photoKeys.Length)
        {
            throw new InvalidOperationException("Every photo must be uploaded by the seller before creating the lot.");
        }

        lot.ReplacePhotos(
            request.SellerId,
            photoKeys.Select((key, index) => new Lot.LotPhotoState(
                key,
                index,
                string.Equals(key, coverPhotoKey, StringComparison.Ordinal))));

        var proofUploads = await GetProofUploadsAsync(request, cancellationToken);
        foreach (var upload in proofUploads)
        {
            lot.AddProofDocument(
                upload.Id,
                request.SellerId,
                upload.DocumentTypeId,
                upload.OriginalFileName,
                upload.StorageKey,
                upload.MimeType,
                upload.SizeBytes,
                upload.CreatedAtUtc);
        }

        await _transactions.ExecuteAsync(async ct =>
        {
            await _lotRepository.AddLotAsync(lot, ct);
            await _pendingUploads.DeleteRangeAsync(uploads, ct);
            await _pendingProofUploads.DeleteRangeAsync(proofUploads, ct);
        }, cancellationToken);

        return lot.Id;
    }

    private async Task<IReadOnlyList<PendingLotProofDocumentUpload>> GetProofUploadsAsync(
        CreateLotCommand request,
        CancellationToken cancellationToken)
    {
        var uploadIds = (request.ProofDocuments ?? [])
            .Select(upload => upload.UploadId)
            .Where(uploadId => uploadId != Guid.Empty)
            .Distinct()
            .ToArray();

        if (uploadIds.Length == 0)
        {
            return [];
        }

        if (uploadIds.Length != request.ProofDocuments!.Count)
        {
            throw new InvalidOperationException("Proof document upload ids must be unique and non-empty.");
        }

        var uploads = await _pendingProofUploads.GetByOwnerAndIdsAsync(
            request.SellerId,
            uploadIds,
            cancellationToken);

        if (uploads.Count != uploadIds.Length)
        {
            throw new InvalidOperationException("Every proof document must be uploaded by the seller before creating the lot.");
        }

        foreach (var upload in uploads)
        {
            var type = await _catalogRepository.GetProofDocumentTypeByIdAsync(upload.DocumentTypeId, cancellationToken);
            if (type is null || !type.IsActive)
            {
                throw new InvalidOperationException("One or more proof document types are not available.");
            }
        }

        return uploads;
    }

    private static string[] NormalizePhotoKeys(IReadOnlyList<string>? photoKeys)
    {
        var normalizedPhotoKeys = (photoKeys ?? [])
            .Select(key => key?.Trim() ?? string.Empty)
            .ToArray();

        if (normalizedPhotoKeys.Length < 5)
        {
            throw new InvalidOperationException("At least 5 photos are required.");
        }

        if (normalizedPhotoKeys.Length > 15)
        {
            throw new InvalidOperationException("Maximum 15 photos are allowed.");
        }

        if (normalizedPhotoKeys.Any(string.IsNullOrWhiteSpace) ||
            normalizedPhotoKeys.Distinct(StringComparer.Ordinal).Count() != normalizedPhotoKeys.Length)
        {
            throw new InvalidOperationException("Photo keys must be unique and non-empty.");
        }

        return normalizedPhotoKeys;
    }

    private static string NormalizeCoverPhotoKey(string? coverPhotoKey, IReadOnlyList<string> photoKeys)
    {
        var normalizedCoverKey = string.IsNullOrWhiteSpace(coverPhotoKey)
            ? photoKeys[0]
            : coverPhotoKey.Trim();

        if (!photoKeys.Contains(normalizedCoverKey, StringComparer.Ordinal))
        {
            throw new InvalidOperationException("Cover photo must be one of the uploaded photos.");
        }

        return normalizedCoverKey;
    }
}
