using Relora.Payments.Application.Interfaces;
using Relora.Payments.Domain.Enums;
using Relora.Shared.Application.Payments;
using Relora.Shared.Domain.Time;

using Stripe;

namespace Relora.Payments.Application.Service;

public sealed class StripePaymentRefundService(
    IPaymentRepository paymentRepository,
    IClock clock) : IPaymentRefundService
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IClock _clock = clock;

    public async Task RefundOrderAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetLatestByOrderIdAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Payment for order {orderId} was not found.");

        if (payment.Status == PaymentStatus.Refunded)
        {
            return;
        }

        if (payment.Status != PaymentStatus.Paid || string.IsNullOrWhiteSpace(payment.StripePaymentIntentId))
        {
            throw new InvalidOperationException("Only a completed Stripe payment can be refunded.");
        }

        var refund = await new RefundService().CreateAsync(
            new RefundCreateOptions
            {
                PaymentIntent = payment.StripePaymentIntentId
            },
            new RequestOptions
            {
                IdempotencyKey = $"refund:{orderId:N}"
            },
            cancellationToken);

        if (!string.Equals(refund.Status, "succeeded", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Stripe has not completed the refund yet. Try again after it is processed.");
        }

        payment.MarkRefunded(refund.Id, _clock.UtcNow);
        await _paymentRepository.UpdatePaymentAsync(payment, cancellationToken);
    }
}
