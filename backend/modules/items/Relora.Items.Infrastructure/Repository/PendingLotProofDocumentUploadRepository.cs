using Relora.Items.Application.Interfaces;
using Relora.Items.Domain;
using Relora.Persistance;

using Microsoft.EntityFrameworkCore;

namespace Relora.Items.Infrastructure.Repository;

public sealed class PendingLotProofDocumentUploadRepository(ReloraDbContext context) : IPendingLotProofDocumentUploadRepository
{
    private readonly ReloraDbContext _context = context;

    public async Task AddAsync(PendingLotProofDocumentUpload upload, CancellationToken cancellationToken)
    {
        await _context.PendingLotProofDocumentUploads.AddAsync(upload, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<PendingLotProofDocumentUpload?> GetByOwnerAndIdAsync(
        Guid ownerId,
        Guid id,
        CancellationToken cancellationToken)
    {
        return _context.PendingLotProofDocumentUploads
            .FirstOrDefaultAsync(upload => upload.OwnerId == ownerId && upload.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<PendingLotProofDocumentUpload>> GetByOwnerAndIdsAsync(
        Guid ownerId,
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken)
    {
        var normalizedIds = ids.Where(id => id != Guid.Empty).Distinct().ToArray();

        if (normalizedIds.Length == 0)
        {
            return [];
        }

        return await _context.PendingLotProofDocumentUploads
            .Where(upload => upload.OwnerId == ownerId && normalizedIds.Contains(upload.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(PendingLotProofDocumentUpload upload, CancellationToken cancellationToken)
    {
        _context.PendingLotProofDocumentUploads.Remove(upload);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<PendingLotProofDocumentUpload> uploads, CancellationToken cancellationToken)
    {
        _context.PendingLotProofDocumentUploads.RemoveRange(uploads);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
