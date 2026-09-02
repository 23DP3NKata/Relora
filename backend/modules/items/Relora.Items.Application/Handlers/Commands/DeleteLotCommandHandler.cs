using Relora.Items.Application.Commands;
using Relora.Items.Application.Interfaces;
using Relora.Shared.Domain.Enums;

using MediatR;
using Relora.Shared.Infrastructure.Media;

namespace Relora.Items.Application.Handlers.Commands;

/// <summary>
/// Represents the delete lot command handler class.
/// </summary>
public sealed class DeleteLotCommandHandler(
    ILotRepository lotRepository,
    IMediaUploader mediaUploader
) : IRequestHandler<DeleteLotCommand>
{
    private readonly ILotRepository _lotRepository = lotRepository;
    private readonly IMediaUploader _mediaUploader = mediaUploader;

    public async Task Handle(DeleteLotCommand command, CancellationToken cancellationToken)
    {
        var lotToDelete = await _lotRepository.GetLotById(command.lotId, cancellationToken);

        if (lotToDelete is null)
        {
            throw new KeyNotFoundException($"Lot {command.lotId} not found.");
        }

        if (lotToDelete.SellerId != command.sellerId)
        {
            throw new UnauthorizedAccessException("Only the seller can delete this lot.");
        }

        if (lotToDelete.Status != LotStatus.Draft)
        {
            throw new InvalidOperationException("Only draft lots can be deleted.");
        }

        foreach (var media in lotToDelete.Media?.Where(x => x.Type == "photo").ToList() ?? [])
        {
            if (!string.IsNullOrWhiteSpace(media.Key))
            {
                await _mediaUploader.DeleteForLotAsync(media.Key);
            }

            lotToDelete.DeletePhoto(media);
        }

        foreach (var document in lotToDelete.ProofDocuments.ToList())
        {
            if (!string.IsNullOrWhiteSpace(document.StorageKey))
            {
                await _mediaUploader.DeleteForLotAsync(document.StorageKey);
            }
        }

        await _lotRepository.DeleteLotAsync(lotToDelete, cancellationToken);
    }
}
