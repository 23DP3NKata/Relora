using Relora.Items.Application.Interfaces;
using Relora.Items.Domain;
using Relora.Persistance;

using Microsoft.EntityFrameworkCore;

namespace Relora.Items.Infrastructure.Repository;

public sealed class PendingLotMediaUploadRepository(ReloraDbContext context) : IPendingLotMediaUploadRepository
{
    private readonly ReloraDbContext _context = context;

    public async Task AddAsync(PendingLotMediaUpload upload, CancellationToken cancellationToken)
    {
        await _context.PendingLotMediaUploads.AddAsync(upload, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<PendingLotMediaUpload?> GetByOwnerAndKeyAsync(
        Guid ownerId,
        string key,
        CancellationToken cancellationToken)
    {
        return _context.PendingLotMediaUploads.FirstOrDefaultAsync(
            upload => upload.OwnerId == ownerId && upload.Key == key.Trim(),
            cancellationToken);
    }

    public async Task<IReadOnlyList<PendingLotMediaUpload>> GetByOwnerAndKeysAsync(
        Guid ownerId,
        IReadOnlyCollection<string> keys,
        CancellationToken cancellationToken)
    {
        var normalizedKeys = keys
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .Select(key => key.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (normalizedKeys.Length == 0)
        {
            return [];
        }

        return await _context.PendingLotMediaUploads
            .Where(upload => upload.OwnerId == ownerId && normalizedKeys.Contains(upload.Key))
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(PendingLotMediaUpload upload, CancellationToken cancellationToken)
    {
        _context.PendingLotMediaUploads.Remove(upload);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<PendingLotMediaUpload> uploads, CancellationToken cancellationToken)
    {
        _context.PendingLotMediaUploads.RemoveRange(uploads);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
