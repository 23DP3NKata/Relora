using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Relora.Payments.Domain.Enums;

namespace Relora.Payments.Domain;
public class Payout
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid SellerId { get; private set; }

    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "eur";

    public string? StripeTransferId { get; private set; }

    public PayoutStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime? LastAttemptAtUtc { get; private set; }
    public int FailureCount { get; private set; }
    public string? LastError { get; private set; }

    public Payout() { }

    public static Payout Create(
        Guid orderId,
        Guid sellerId,
        decimal amount,
        string currency
    )
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException("Order id is required.");
        }

        if (sellerId == Guid.Empty)
        {
            throw new ArgumentException("Seller id is required.");
        }

        if (amount <= 0)
        {
            throw new ArgumentException("Payout amount must be greater than zero.");
        }

        return new Payout
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            SellerId = sellerId,
            Amount = amount,
            Currency = NormalizeCurrency(currency),
            Status = PayoutStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkSucceeded(string stripeTransferId, DateTime paidAtUtc)
    {
        if (string.IsNullOrWhiteSpace(stripeTransferId))
        {
            throw new ArgumentException("Stripe transfer id is required.");
        }

        StripeTransferId = stripeTransferId.Trim();
        PaidAt = paidAtUtc;
        LastAttemptAtUtc = paidAtUtc;
        LastError = null;
        Status = PayoutStatus.Success;
    }

    public void MarkFailed(string error, DateTime attemptedAtUtc)
    {
        var normalizedError = string.IsNullOrWhiteSpace(error)
            ? "Payout processing failed."
            : error.Trim();

        LastAttemptAtUtc = attemptedAtUtc;
        FailureCount++;
        LastError = normalizedError[..Math.Min(normalizedError.Length, 1000)];
        Status = PayoutStatus.Failed;
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
