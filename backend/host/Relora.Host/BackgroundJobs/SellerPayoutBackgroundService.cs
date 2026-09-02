using Relora.Orders.Domain.Enums;
using Relora.Payments.Domain;
using Relora.Payments.Domain.Enums;
using Relora.Persistance;
using Relora.Shared.Domain.Time;

using Microsoft.EntityFrameworkCore;

using Stripe;

namespace Relora.Host.BackgroundJobs;

public sealed class SellerPayoutBackgroundService(
    IServiceScopeFactory scopeFactory,
    IClock clock,
    ILogger<SellerPayoutBackgroundService> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IClock _clock = clock;
    private readonly ILogger<SellerPayoutBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingPayouts(stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to process seller payouts.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task ProcessPendingPayouts(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ReloraDbContext>();
        var now = _clock.UtcNow;

        var completedOrders = await db.Orders
            .Where(order => order.Status == OrderStatus.Completed)
            .ToListAsync(cancellationToken);

        foreach (var order in completedOrders)
        {
            var payout = await db.Payouts
                .FirstOrDefaultAsync(item => item.OrderId == order.Id, cancellationToken);

            if (payout?.Status == PayoutStatus.Success)
            {
                continue;
            }

            payout ??= Relora.Payments.Domain.Payout.Create(order.Id, order.SellerId, order.TotalPrice, order.Currency);
            if (db.Entry(payout).State == EntityState.Detached)
            {
                db.Payouts.Add(payout);
            }

            var account = await db.SellerPaymentAccounts
                .FirstOrDefaultAsync(item => item.SellerId == order.SellerId, cancellationToken);
            var payment = await db.Payments
                .Where(item => item.OrderId == order.Id && item.Status == PaymentStatus.Paid)
                .OrderByDescending(item => item.PaidAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (account?.Status != SellerPaymentAccountStatus.Ready ||
                payment is null ||
                string.IsNullOrWhiteSpace(payment.StripePaymentIntentId))
            {
                payout.MarkFailed("Seller payout account or completed payment is unavailable.", now);
                await db.SaveChangesAsync(cancellationToken);
                continue;
            }

            try
            {
                var paymentIntent = await new PaymentIntentService().GetAsync(
                    payment.StripePaymentIntentId,
                    cancellationToken: cancellationToken);

                if (string.IsNullOrWhiteSpace(paymentIntent.LatestChargeId))
                {
                    throw new InvalidOperationException("Stripe payment intent has no settled charge.");
                }

                var transfer = await new TransferService().CreateAsync(
                    new TransferCreateOptions
                    {
                        Amount = ToCents(payout.Amount),
                        Currency = payout.Currency.ToLowerInvariant(),
                        Destination = account.StripeConnectedAccountId,
                        SourceTransaction = paymentIntent.LatestChargeId,
                        TransferGroup = $"ORDER_{order.Id}"
                    },
                    new RequestOptions { IdempotencyKey = $"payout:{order.Id:N}" },
                    cancellationToken);

                payout.MarkSucceeded(transfer.Id, now);
                await db.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                payout.MarkFailed(exception.Message, now);
                await db.SaveChangesAsync(cancellationToken);
                _logger.LogWarning(exception, "Payout for order {OrderId} was not completed.", order.Id);
            }
        }
    }

    private static long ToCents(decimal amount)
    {
        return (long)Math.Round(amount * 100, MidpointRounding.AwayFromZero);
    }
}
