using Relora.Orders.Domain.Enums;
using Relora.Persistance;
using Relora.Shared.Domain.Enums;
using Relora.Shared.Domain.Time;
using Relora.Shared.Infrastructure.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Relora.Host.BackgroundJobs;

/// <summary>
/// Represents the order payment expiration background service class.
/// </summary>
public sealed class OrderPaymentExpirationBackgroundService(
    IServiceScopeFactory scopeFactory,
    IClock clock,
    ILogger<OrderPaymentExpirationBackgroundService> logger)
    : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IClock _clock = clock;
    private readonly ILogger<OrderPaymentExpirationBackgroundService> _logger = logger;

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
                await ExpireOrders(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to expire overdue orders.");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    /// <summary>
    /// Performs the expire orders operation.
    /// </summary>
    /// <param name="ct">Ct.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task ExpireOrders(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ReloraDbContext>();
        var domainEventDispatcher = scope.ServiceProvider.GetRequiredService<IDomainEventDispatcher>();

        var now = _clock.UtcNow;

        var pendingOrders = await db.Orders
            .Where(x => x.Status == OrderStatus.PendingPayment && x.PaymentDeadlineUtc < now)
            .ToListAsync(ct);

        foreach (var order in pendingOrders)
        {
            order.MarkAsExpired(now);

            var auction = await db.Auctions
                .FirstOrDefaultAsync(x => x.Id == order.AuctionId, ct);

            if (auction?.LotId is Guid lotId)
            {
                var lot = await db.Lots
                    .FirstOrDefaultAsync(x => x.Id == lotId, ct);

                if (lot?.Status == LotStatus.Sold)
                {
                    lot.MarkUnsold();
                }
            }

            await domainEventDispatcher.DispatchAsync(order.DomainEvents, ct);
            order.ClearDomainEvents();
        }

        if (pendingOrders.Count > 0)
        {
            await db.SaveChangesAsync(ct);
            _logger.LogInformation("Expired {Count} overdue orders.", pendingOrders.Count);
        }
    }
}
