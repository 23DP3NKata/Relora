using Relora.Orders.Domain.Enums;
using Relora.Shared.Domain.Abstractions;
using Relora.Orders.Domain.Events;

namespace Relora.Orders.Domain;

/// <summary>
/// Represents the order class.
/// </summary>
public class Order : AggregateRoot<Guid>
{
    public static readonly TimeSpan PaymentWindow = TimeSpan.FromDays(3);
    public static readonly TimeSpan DeliveryIssueWindow = TimeSpan.FromDays(14);

    /// <summary>
    /// Gets or sets the auction id used by this type.
    /// </summary>
    public Guid AuctionId { get; private set; }
    /// <summary>
    /// Gets or sets the seller id used by this type.
    /// </summary>
    public Guid SellerId { get; private set; }
    /// <summary>
    /// Gets or sets the buyer id used by this type.
    /// </summary>
    public Guid BuyerId { get; private set; }

    /// <summary>
    /// Gets or sets the status used by this type.
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// Gets or sets the price used by this type.
    /// </summary>
    public decimal Price { get; private set; }
    public decimal ShippingPrice { get; private set; }
    public decimal TotalPrice => Price + ShippingPrice;
    /// <summary>
    /// Gets or sets the currency used by this type.
    /// </summary>
    public string Currency { get; private set; }
    public string ShippingCurrency { get; private set; }
    public int ShippingHandlingDays { get; private set; }
    public DateTime? ShipByUtc { get; private set; }
    public DateTime? ShippedAtUtc { get; private set; }
    public DateTime? DeliveredAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public DateTime? DeliveryIssueAvailableFromUtc { get; private set; }
    public DateTime? DeliveryIssueReportedAtUtc { get; private set; }
    public string? DeliveryIssueReason { get; private set; }
    public string? CarrierName { get; private set; }
    public string? TrackingNumber { get; private set; }
    public string? ShippingFullName { get; private set; }
    public string? ShippingCountryCode { get; private set; }
    public string? ShippingCountry { get; private set; }
    public string? ShippingCity { get; private set; }
    public string? ShippingPostalCode { get; private set; }
    public string? ShippingAddressLine1 { get; private set; }
    public string? ShippingAddressLine2 { get; private set; }
    public string? ShippingPhone { get; private set; }
    public string ShipsToCountries { get; private set; }
    public string ShippingOriginCountry { get; private set; }
    public bool HasShippingAddress => !string.IsNullOrWhiteSpace(ShippingAddressLine1);

    /// <summary>
    /// Gets or sets the payment deadline utc used by this type.
    /// </summary>
    public DateTime PaymentDeadlineUtc { get; private set; }

    /// <summary>
    /// Gets or sets the stripe checkout session id used by this type.
    /// </summary>
    public string? StripeCheckoutSessionId { get; private set; }
    public int CheckoutSessionVersion { get; private set; }
    /// <summary>
    /// Gets or sets the paid at utc used by this type.
    /// </summary>
    public DateTime? PaidAtUtc { get; private set; }

    private Order(Guid id) : base(id) { }

    public static Order Create
    (
        Guid auctionId,
        Guid sellerId,
        Guid buyerId,
        decimal price,
        string currency,
        DateTime deadlineUtc,
        decimal shippingPrice,
        string shippingCurrency,
        string shippingOriginCountry,
        string shipsToCountries,
        int shippingHandlingDays
    )
    {
        if (price <= 0)
        {
            throw new ArgumentException("Order price must be greater than zero.");
        }

        if (shippingPrice < 0)
        {
            throw new ArgumentException("Shipping price cannot be negative.");
        }

        if (shippingHandlingDays < 1 || shippingHandlingDays > 30)
        {
            throw new ArgumentException("Shipping handling time must be between 1 and 30 business days.");
        }

        var normalizedCurrency = NormalizeCurrency(currency);
        var normalizedShippingCurrency = NormalizeCurrency(shippingCurrency);

        if (!string.Equals(normalizedCurrency, normalizedShippingCurrency, StringComparison.Ordinal))
        {
            throw new ArgumentException("Order and shipping currencies must match.");
        }

        var order = new Order(Guid.NewGuid())
        {
            AuctionId = auctionId,
            SellerId = sellerId,
            BuyerId = buyerId,
            Price = price,
            Currency = normalizedCurrency,
            ShippingPrice = shippingPrice,
            ShippingCurrency = normalizedShippingCurrency,
            ShippingOriginCountry = NormalizeRequired(shippingOriginCountry, "Shipping origin country"),
            ShipsToCountries = NormalizeRequired(shipsToCountries, "Shipping destinations"),
            ShippingHandlingDays = shippingHandlingDays,
            PaymentDeadlineUtc = deadlineUtc,
            Status = OrderStatus.PendingPayment
        };

        order.AddDomainEvent(new OrderCreatedDomainEvent(
            order.Id,
            auctionId,
            sellerId,
            buyerId,
            OrderStatus.PendingPayment,
            price,
            currency,
            deadlineUtc
        ));

        return order;
    }

    /// <summary>
    /// Marks as paid.
    /// </summary>
    /// <param name="paidAtUtc">Date and time for the operation.</param>
    public void MarkAsPaid(DateTime paidAtUtc)
    {
        if (Status != OrderStatus.PendingPayment)
        {
            throw new InvalidOperationException("Order is not payable.");
        }

        if (paidAtUtc > PaymentDeadlineUtc)
        {
            throw new InvalidOperationException("Payment deadline expired.");
        }

        Status = OrderStatus.AwaitingShipment;
        PaidAtUtc = paidAtUtc;
        ShipByUtc = AddBusinessDays(paidAtUtc, ShippingHandlingDays);
        AddDomainEvent(new OrderPaidDomainEvent(Id, paidAtUtc));
    }

    /// <summary>
    /// Marks as failed.
    /// </summary>
    /// <param name="paidAtUtc">Date and time for the operation.</param>
    public void MarkAsFailed(DateTime paidAtUtc)
    {
        if (Status != OrderStatus.PendingPayment)
        {
            throw new InvalidOperationException("Order is not payable.");
        }

        if (paidAtUtc > PaymentDeadlineUtc)
        {
            throw new InvalidOperationException("Payment deadline expired.");
        }

        Status = OrderStatus.Failed;
        PaidAtUtc = paidAtUtc;
    }

    /// <summary>
    /// Marks as expired.
    /// </summary>
    /// <param name="expiredAtUtc">Expired at utc.</param>
    public void MarkAsExpired(DateTime expiredAtUtc)
    {
        if (Status != OrderStatus.PendingPayment)
        {
            return;
        }

        Status = OrderStatus.PaymentExpired;
        AddDomainEvent(new OrderExpiderDomainEvent(Id,expiredAtUtc));
    }

    /// <summary>
    /// Cancels the operation.
    /// </summary>
    public void Cancel()
    {
        if (Status == OrderStatus.Paid)
        {
            throw new InvalidOperationException("Paid order cannot be cancelled.");
        }

        Status = OrderStatus.Cancelled;
    }

    public void SetShippingAddress(
        string fullName,
        string countryCode,
        string country,
        string city,
        string postalCode,
        string addressLine1,
        string? addressLine2,
        string? phone)
    {
        if (Status != OrderStatus.PendingPayment)
        {
            throw new InvalidOperationException("Shipping address can only be changed before payment.");
        }

        ShippingFullName = NormalizeRequired(fullName, "Recipient name");
        ShippingCountryCode = NormalizeRequired(countryCode, "Country code").ToUpperInvariant();
        ShippingCountry = NormalizeRequired(country, "Country");
        ShippingCity = NormalizeRequired(city, "City");
        ShippingPostalCode = NormalizeRequired(postalCode, "Postal code");
        ShippingAddressLine1 = NormalizeRequired(addressLine1, "Address line 1");
        ShippingAddressLine2 = string.IsNullOrWhiteSpace(addressLine2) ? null : addressLine2.Trim();
        ShippingPhone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();

        if (!CanShipTo(ShippingCountryCode) && !CanShipTo(ShippingCountry))
        {
            throw new InvalidOperationException("Seller does not ship to this destination.");
        }
    }

    public void MarkAsShipped(string carrierName, string trackingNumber, DateTime shippedAtUtc)
    {
        if (Status != OrderStatus.AwaitingShipment && Status != OrderStatus.Paid)
        {
            throw new InvalidOperationException("Only paid orders awaiting shipment can be shipped.");
        }

        CarrierName = NormalizeRequired(carrierName, "Carrier");
        TrackingNumber = NormalizeRequired(trackingNumber, "Tracking number");
        ShippedAtUtc = shippedAtUtc;
        DeliveryIssueAvailableFromUtc = shippedAtUtc.Add(DeliveryIssueWindow);
        Status = OrderStatus.Shipped;
        AddDomainEvent(new OrderShippedDomainEvent(Id, BuyerId, CarrierName, TrackingNumber, shippedAtUtc));
    }

    public void ConfirmReceived(DateTime receivedAtUtc)
    {
        if (Status != OrderStatus.Shipped)
        {
            throw new InvalidOperationException("Only shipped orders can be confirmed as received.");
        }

        DeliveredAtUtc = receivedAtUtc;
        CompletedAtUtc = receivedAtUtc;
        Status = OrderStatus.Completed;
        AddDomainEvent(new OrderCompletedDomainEvent(Id, BuyerId, SellerId, receivedAtUtc));
    }

    public void OpenDispute(DateTime disputedAtUtc)
    {
        if (Status is
            OrderStatus.PendingPayment or
            OrderStatus.PaymentExpired or
            OrderStatus.Cancelled or
            OrderStatus.Failed or
            OrderStatus.Refunded)
        {
            throw new InvalidOperationException("This order cannot be disputed.");
        }

        if (Status == OrderStatus.Disputed)
        {
            throw new InvalidOperationException("This order is already disputed.");
        }

        Status = OrderStatus.Disputed;
    }

    public void MarkAsRefunded(DateTime refundedAtUtc)
    {
        if (Status != OrderStatus.Disputed)
        {
            throw new InvalidOperationException("Only disputed orders can be refunded by dispute resolution.");
        }

        Status = OrderStatus.Refunded;
    }

    public void CompleteAfterDispute(DateTime completedAtUtc)
    {
        if (Status != OrderStatus.Disputed)
        {
            throw new InvalidOperationException("Only disputed orders can be completed by dispute resolution.");
        }

        DeliveredAtUtc ??= completedAtUtc;
        CompletedAtUtc = completedAtUtc;
        Status = OrderStatus.Completed;
        AddDomainEvent(new OrderCompletedDomainEvent(Id, BuyerId, SellerId, completedAtUtc));
    }

    public void ReportNotDelivered(string? reason, DateTime reportedAtUtc)
    {
        if (Status != OrderStatus.Shipped)
        {
            throw new InvalidOperationException("Only shipped orders can be reported as not delivered.");
        }

        if (DeliveryIssueAvailableFromUtc != null && reportedAtUtc < DeliveryIssueAvailableFromUtc.Value)
        {
            throw new InvalidOperationException("The delivery protection window has not opened yet.");
        }

        DeliveryIssueReportedAtUtc = reportedAtUtc;
        DeliveryIssueReason = string.IsNullOrWhiteSpace(reason)
            ? null
            : reason.Trim();
        Status = OrderStatus.BuyerProtection;

        AddDomainEvent(new OrderDeliveryIssueReportedDomainEvent(
            Id,
            BuyerId,
            SellerId,
            DeliveryIssueReason,
            reportedAtUtc));
    }

    /// <summary>
    /// Sets stripe session.
    /// </summary>
    /// <param name="sessionId">Identifier of session.</param>
    public void SetStripeSession(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw new ArgumentException("Stripe checkout session id is required.");
        }

        StripeCheckoutSessionId = sessionId;
    }

    public string GetCheckoutIdempotencyKey()
    {
        return $"checkout:{Id:N}:{CheckoutSessionVersion}";
    }

    public void ClearStripeSession(string sessionId)
    {
        if (StripeCheckoutSessionId == sessionId)
        {
            StripeCheckoutSessionId = null;
            CheckoutSessionVersion++;
        }
    }

    private bool CanShipTo(string destination)
    {
        if (string.IsNullOrWhiteSpace(destination))
        {
            return false;
        }

        var normalizedDestination = destination.Trim();

        return ShipsToCountries
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(country =>
                string.Equals(country, normalizedDestination, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(country, "Worldwide", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(country, "World", StringComparison.OrdinalIgnoreCase));
    }

    private static DateTime AddBusinessDays(DateTime start, int businessDays)
    {
        var date = start;
        var addedDays = 0;

        while (addedDays < businessDays)
        {
            date = date.AddDays(1);

            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                continue;
            }

            addedDays++;
        }

        return date;
    }

    private static string NormalizeRequired(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{fieldName} is required.");
        }

        return value.Trim();
    }

    private static string NormalizeCurrency(string value)
    {
        var currency = NormalizeRequired(value, "Currency").ToUpperInvariant();

        if (currency.Length != 3)
        {
            throw new ArgumentException("Currency must be a 3-letter ISO code.");
        }

        return currency;
    }
}
