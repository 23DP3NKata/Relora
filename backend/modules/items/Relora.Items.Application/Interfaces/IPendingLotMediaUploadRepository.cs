using Relora.Items.Domain;

namespace Relora.Items.Application.Interfaces;

public interface IPendingLotMediaUploadRepository
{
    Task AddAsync(PendingLotMediaUpload upload, CancellationToken cancellationToken);
    Task<PendingLotMediaUpload?> GetByOwnerAndKeyAsync(Guid ownerId, string key, CancellationToken cancellationToken);
    Task<IReadOnlyList<PendingLotMediaUpload>> GetByOwnerAndKeysAsync(
        Guid ownerId,
        IReadOnlyCollection<string> keys,
        CancellationToken cancellationToken);
    Task DeleteAsync(PendingLotMediaUpload upload, CancellationToken cancellationToken);
    Task DeleteRangeAsync(IEnumerable<PendingLotMediaUpload> uploads, CancellationToken cancellationToken);
}
