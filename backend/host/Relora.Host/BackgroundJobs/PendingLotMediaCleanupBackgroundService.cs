using Relora.Persistance;
using Relora.Shared.Domain.Time;
using Relora.Shared.Infrastructure.Media;

using Microsoft.EntityFrameworkCore;

namespace Relora.Host.BackgroundJobs;

public sealed class PendingLotMediaCleanupBackgroundService(
    IServiceScopeFactory scopeFactory,
    IClock clock,
    ILogger<PendingLotMediaCleanupBackgroundService> logger) : BackgroundService
{
    private static readonly TimeSpan UploadLifetime = TimeSpan.FromHours(24);
    private static readonly TimeSpan CleanupInterval = TimeSpan.FromHours(1);
    private const int PendingUploadsBatchSize = 100;

    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IClock _clock = clock;
    private readonly ILogger<PendingLotMediaCleanupBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredUploads(stoppingToken);
                await CleanupLegacyOrphans(stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to clean up unused lot media.");
            }

            await Task.Delay(CleanupInterval, stoppingToken);
        }
    }

    private async Task CleanupExpiredUploads(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ReloraDbContext>();
        var mediaUploader = scope.ServiceProvider.GetRequiredService<IMediaUploader>();
        var cutoffUtc = _clock.UtcNow.Subtract(UploadLifetime);

        var expiredUploads = await db.PendingLotMediaUploads
            .Where(upload => upload.CreatedAtUtc <= cutoffUtc)
            .OrderBy(upload => upload.CreatedAtUtc)
            .Take(PendingUploadsBatchSize)
            .ToListAsync(cancellationToken);

        foreach (var upload in expiredUploads)
        {
            try
            {
                db.PendingLotMediaUploads.Remove(upload);
                await db.SaveChangesAsync(cancellationToken);

                await mediaUploader.DeleteForLotAsync(upload.Key);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(
                    exception,
                    "Could not remove expired pending media upload {MediaKey}.",
                    upload.Key);
            }
        }

        var expiredProofUploads = await db.PendingLotProofDocumentUploads
            .Where(upload => upload.CreatedAtUtc <= cutoffUtc)
            .OrderBy(upload => upload.CreatedAtUtc)
            .Take(PendingUploadsBatchSize)
            .ToListAsync(cancellationToken);

        foreach (var upload in expiredProofUploads)
        {
            try
            {
                db.PendingLotProofDocumentUploads.Remove(upload);
                await db.SaveChangesAsync(cancellationToken);

                await mediaUploader.DeleteForLotAsync(upload.StorageKey);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(
                    exception,
                    "Could not remove expired pending proof document {MediaKey}.",
                    upload.StorageKey);
            }
        }
    }

    private async Task CleanupLegacyOrphans(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ReloraDbContext>();
        var mediaUploader = scope.ServiceProvider.GetRequiredService<IMediaUploader>();
        var cutoffUtc = _clock.UtcNow.Subtract(UploadLifetime);

        var attachedKeys = await db.Lots
            .AsNoTracking()
            .SelectMany(lot => lot.Media.Select(media => media.Key))
            .ToListAsync(cancellationToken);
        var pendingKeys = await db.PendingLotMediaUploads
            .AsNoTracking()
            .Select(upload => upload.Key)
            .ToListAsync(cancellationToken);
        var referencedKeys = attachedKeys
            .Concat(pendingKeys)
            .ToHashSet(StringComparer.Ordinal);

        var oldKeys = await mediaUploader.GetKeysOlderThanAsync("lots/", cutoffUtc, cancellationToken);

        foreach (var key in oldKeys.Where(key => !referencedKeys.Contains(key)))
        {
            try
            {
                await mediaUploader.DeleteForLotAsync(key);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Could not remove orphaned media object {MediaKey}.", key);
            }
        }

        var attachedProofKeys = await db.Lots
            .AsNoTracking()
            .SelectMany(lot => lot.ProofDocuments.Select(document => document.StorageKey))
            .ToListAsync(cancellationToken);
        var pendingProofKeys = await db.PendingLotProofDocumentUploads
            .AsNoTracking()
            .Select(upload => upload.StorageKey)
            .ToListAsync(cancellationToken);
        var referencedProofKeys = attachedProofKeys
            .Concat(pendingProofKeys)
            .ToHashSet(StringComparer.Ordinal);

        var oldProofKeys = await mediaUploader.GetKeysOlderThanAsync("proof-origin/", cutoffUtc, cancellationToken);

        foreach (var key in oldProofKeys.Where(key => !referencedProofKeys.Contains(key)))
        {
            try
            {
                await mediaUploader.DeleteForLotAsync(key);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Could not remove orphaned proof document {MediaKey}.", key);
            }
        }
    }
}
