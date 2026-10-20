using Relora.Items.Application.Commands;
using Relora.Items.Application.Interfaces;
using Relora.Items.Application.Models;
using Relora.Items.Application.Services;
using Relora.Items.Domain;
using Relora.Items.Domain.Enums;
using Relora.Shared.Domain.Persistence;
using Relora.Shared.Domain.Time;
using Relora.Shared.Domain.ValueObjects;
using Relora.Shared.Infrastructure.Media;

using MediatR;

namespace Relora.Items.Application.Handlers.Commands;

/// <summary>
/// Represents the edit lot command handler class.
/// </summary>
public sealed class EditLotCommandHandler : IRequestHandler<EditLotCommand>
{
    private readonly ILotRepository _lotRepository;
    private readonly ILotCatalogRepository _catalogRepository;
    private readonly IPendingLotMediaUploadRepository _pendingMediaUploads;
    private readonly ITransactionRunner _transactions;
    private readonly IMediaUploader _mediaUploader;
    private readonly IClock _clock;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditLotCommandHandler"/> class.
    /// </summary>
    public EditLotCommandHandler(
        ILotRepository lotRepository,
        ILotCatalogRepository catalogRepository,
        IPendingLotMediaUploadRepository pendingMediaUploads,
        ITransactionRunner transactions,
        IMediaUploader mediaUploader,
        IClock clock)
    {
        _lotRepository = lotRepository;
        _catalogRepository = catalogRepository;
        _pendingMediaUploads = pendingMediaUploads;
        _transactions = transactions;
        _mediaUploader = mediaUploader;
        _clock = clock;
    }

    /// <summary>
    /// Handles the operation.
    /// </summary>
    public async Task Handle(EditLotCommand request, CancellationToken cancellationToken)
    {
        var lot = await _lotRepository.GetLotById(request.id, cancellationToken);

        if (lot is null)
        {
            throw new KeyNotFoundException($"Lot {request.id} not found.");
        }

        var categoryId = request.categoryId;
        if (!categoryId.HasValue && lot.Category == request.category && lot.CategoryId != Guid.Empty)
        {
            categoryId = lot.CategoryId;
        }

        var primaryColorId = request.primaryColorId;
        if (!primaryColorId.HasValue &&
            string.Equals(lot.Color, request.color, StringComparison.OrdinalIgnoreCase) &&
            lot.PrimaryColorId != Guid.Empty)
        {
            primaryColorId = lot.PrimaryColorId;
        }

        var materialInputs = request.materials ?? lot.Materials
            .Select(material => new LotMaterialInput(material.MaterialId, material.Percentage, material.OtherName))
            .ToList();

        var measurementInputs = request.measurements ?? lot.Measurements
            .Select(measurement => new LotMeasurementInput(measurement.Key, measurement.Value, measurement.Unit))
            .ToList();

        var catalogInput = await LotCatalogInputResolver.ResolveAsync(
            _catalogRepository,
            categoryId,
            request.department,
            request.category,
            request.gender,
            primaryColorId,
            request.color,
            materialInputs,
            measurementInputs,
            cancellationToken);

        lot.Edit(
            request.sellerId,
            request.title,
            request.description,
            request.price,
            catalogInput.CategoryId,
            catalogInput.Department,
            catalogInput.LegacyCategory,
            catalogInput.LegacyGender,
            request.size,
            request.brand,
            request.condition,
            catalogInput.PrimaryColorId,
            request.color,
            request.country,
            request.city,
            request.modelName,
            request.acquisitionYear,
            request.productionYear,
            request.isVintage || request.category == LotCategory.Vintage,
            request.vintageNotes,
            request.age,
            request.style,
            request.shippingPrice,
            request.shippingCurrency,
            request.shippingOriginCountry,
            request.shipsToCountries,
            request.shippingHandlingDays,
            catalogInput.Materials,
            lot.SecondaryColors.Select(color => color.ColorId),
            catalogInput.Measurements,
            _clock.UtcNow);

        var photoChanges = await ApplyPhotoChangesAsync(lot, request, cancellationToken);

        await _transactions.ExecuteAsync(async ct =>
        {
            await _lotRepository.SaveLotAsync(lot, ct);

            if (photoChanges.PendingUploads.Count > 0)
            {
                await _pendingMediaUploads.DeleteRangeAsync(photoChanges.PendingUploads, ct);
            }
        }, cancellationToken);

        foreach (var key in photoChanges.RemovedPhotoKeys)
        {
            try
            {
                await _mediaUploader.DeleteForLotAsync(key);
            }
            catch
            {
                // The database update is already complete; storage cleanup can be retried by operational maintenance.
            }
        }
    }

    private async Task<PhotoChangeSet> ApplyPhotoChangesAsync(
        Lot lot,
        EditLotCommand request,
        CancellationToken cancellationToken)
    {
        var photoKeys = NormalizeOptionalPhotoKeys(request.photoKeys);
        if (photoKeys is null)
        {
            return new PhotoChangeSet([], []);
        }

        var existingPhotoKeys = lot.Media
            .Where(media => media.Type == "photo")
            .Select(media => media.Key)
            .ToHashSet(StringComparer.Ordinal);

        var newPhotoKeys = photoKeys
            .Where(key => !existingPhotoKeys.Contains(key))
            .ToArray();

        var removedPhotoKeys = existingPhotoKeys
            .Where(key => !photoKeys.Contains(key, StringComparer.Ordinal))
            .ToArray();

        var uploads = await _pendingMediaUploads.GetByOwnerAndKeysAsync(
            request.sellerId,
            newPhotoKeys,
            cancellationToken);

        if (uploads.Count != newPhotoKeys.Length)
        {
            throw new InvalidOperationException("Every new photo must be uploaded by the seller before editing the lot.");
        }

        var coverPhotoKey = NormalizeCoverPhotoKey(request.coverPhotoKey, photoKeys);
        lot.ReplacePhotos(
            request.sellerId,
            photoKeys.Select((key, index) => new Lot.LotPhotoState(
                key,
                index,
                string.Equals(key, coverPhotoKey, StringComparison.Ordinal))));

        return new PhotoChangeSet(uploads, removedPhotoKeys);
    }

    private static string[]? NormalizeOptionalPhotoKeys(IReadOnlyList<string>? photoKeys)
    {
        if (photoKeys is null)
        {
            return null;
        }

        var normalizedPhotoKeys = photoKeys
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
            throw new InvalidOperationException("Cover photo must be one of the listing photos.");
        }

        return normalizedCoverKey;
    }

    private sealed record PhotoChangeSet(
        IReadOnlyList<PendingLotMediaUpload> PendingUploads,
        IReadOnlyList<string> RemovedPhotoKeys);
}
