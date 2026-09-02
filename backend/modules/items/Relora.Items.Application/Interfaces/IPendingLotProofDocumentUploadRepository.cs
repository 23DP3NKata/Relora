using Relora.Items.Domain;

namespace Relora.Items.Application.Interfaces;

public interface IPendingLotProofDocumentUploadRepository
{
    Task AddAsync(PendingLotProofDocumentUpload upload, CancellationToken cancellationToken);
    Task<PendingLotProofDocumentUpload?> GetByOwnerAndIdAsync(Guid ownerId, Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<PendingLotProofDocumentUpload>> GetByOwnerAndIdsAsync(
        Guid ownerId,
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken);
    Task DeleteAsync(PendingLotProofDocumentUpload upload, CancellationToken cancellationToken);
    Task DeleteRangeAsync(IEnumerable<PendingLotProofDocumentUpload> uploads, CancellationToken cancellationToken);
}
