using Relora.Auctions.Application.Commands;
using Relora.Shared.Domain.Enums;
using Relora.Shared.Domain.Time;
using Relora.Persistance;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Relora.Host.BackgroundJobs;

/// <summary>
/// Represents the auction auto stop background service class.
/// </summary>
public sealed class AuctionAutoStopBackgroundService(
    IServiceScopeFactory scopeFactory,
    IClock clock,
    ILogger<AuctionAutoStopBackgroundService> logger)
    : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IClock _clock = clock;
    private readonly ILogger<AuctionAutoStopBackgroundService> _logger = logger;

    /// <summary>
    /// Performs the execute async operation.
    /// </summary>
    /// <param name="stoppingToken">Stopping token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await StopExpiredAuctions(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to auto-stop expired auctions.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    /// <summary>
    /// Stops expired auctions.
    /// </summary>
    /// <param name="ct">Ct.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task StopExpiredAuctions(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ReloraDbContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var now = _clock.UtcNow;

        var expiredAuctionIds = await db.Auctions
            .AsNoTracking()
            .Where(a => a.Status == AuctionStatus.Active && a.EndDate != null && a.EndDate <= now)
            .Select(a => a.Id)
            .ToListAsync(ct);

        foreach (var auctionId in expiredAuctionIds)
        {
            try
            {
                await mediator.Send(new StopAuctionCommand(auctionId, IsSystem: true), ct);
                _logger.LogInformation("Auction {AuctionId} auto-stopped.", auctionId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not auto-stop auction {AuctionId}.", auctionId);
            }
        }
    }
}
