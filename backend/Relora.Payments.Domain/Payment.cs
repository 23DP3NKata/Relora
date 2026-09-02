using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Relora.Payments.Domain.Enums;

namespace Relora.Payments.Domain;
public class Payment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; private set; }
    public Guid BuyerId { get; private set; }

    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "eur";

    public string Provider { get; private set; } = "Stripe";
    public string StripeSessionId { get; private set; } = null!;
    public string? StripePaymentIntentId { get; private set; }
    public string? StripeRefundId { get; private set; }

    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime? RefundedAt { get; private set; }

    public Payment() { }

    public static Payment Create(
        Guid orderId,
        Guid buyerId,
        decimal amount,
        string currency,
        string stripeSessionId
    )
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException("Order id is required.");
        }

        if (buyerId == Guid.Empty)
        {
            throw new ArgumentException("Buyer id is required.");
        }

        if (amount <= 0)
        {
            throw new ArgumentException("Payment amount must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(stripeSessionId))
        {
            throw new ArgumentException("Stripe checkout session id is required.");
        }

        return new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            BuyerId = buyerId,
            Amount = amount,
            Currency = NormalizeCurrency(currency),
            StripeSessionId = stripeSessionId.Trim(),
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkPaid(string paymentIntentId, DateTime paidAtUtc)
    {
        if (Status == PaymentStatus.Paid)
        {
            return;
        }

        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidOperationException("Only pending payments can be marked as paid.");
        }

        if (string.IsNullOrWhiteSpace(paymentIntentId))
        {
            throw new ArgumentException("Stripe payment intent id is required.");
        }

        StripePaymentIntentId = paymentIntentId.Trim();
        PaidAt = paidAtUtc;
        Status = PaymentStatus.Paid;
    }

    public void MarkFailed()
    {
        if (Status == PaymentStatus.Pending)
        {
            Status = PaymentStatus.Failed;
        }
    }

    public void MarkCancelled()
    {
        if (Status == PaymentStatus.Pending)
        {
            Status = PaymentStatus.Cancelled;
        }
    }

    public void MarkRefunded(string refundId, DateTime refundedAtUtc)
    {
        if (Status == PaymentStatus.Refunded)
        {
            return;
        }

        if (Status != PaymentStatus.Paid)
        {
            throw new InvalidOperationException("Only paid payments can be refunded.");
        }

        if (string.IsNullOrWhiteSpace(refundId))
        {
            throw new ArgumentException("Stripe refund id is required.");
        }

        StripeRefundId = refundId.Trim();
        RefundedAt = refundedAtUtc;
        Status = PaymentStatus.Refunded;
    }

    private static string NormalizeCurrency(string currency)
    {
        var normalizedCurrency = currency?.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(normalizedCurrency) ||
            normalizedCurrency.Length != 3 ||
            !normalizedCurrency.All(char.IsLetter))
        {
            throw new ArgumentException("Currency must be a three-letter ISO code.");
        }

        return normalizedCurrency;
    }
}
