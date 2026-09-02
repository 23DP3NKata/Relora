using Relora.Orders.Domain.Enums;
using Relora.Shared.Domain.Abstractions;

namespace Relora.Orders.Domain.Events;
/// <summary>
/// Represents the order created domain event class.
/// </summary>
public class OrderCreatedDomainEvent : IDomainEvent
{
    public Guid OrderId { get; set; }

    /// <summary>
    /// Gets or sets the auction id used by this type.
    /// </summary>
    public Guid AuctionId { get; set; }
    /// <summary>
    /// Gets or sets the seller id used by this type.
    /// </summary>
    public Guid SellerId { get; set; }
    /// <summary>
    /// Gets or sets the buyer id used by this type.
    /// </summary>
    public Guid BuyerId { get; set; }

    /// <summary>
    /// Gets or sets the status used by this type.
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the price used by this type.
    /// </summary>
    public decimal Price { get; set; }
    /// <summary>
    /// Gets or sets the currency used by this type.
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// Gets or sets the payment deadline utc used by this type.
    /// </summary>
    public DateTime PaymentDeadlineUtc { get; set; }
    /// <summary>
    /// Gets or sets the occurred at used by this type.
    /// </summary>
    public DateTime OccurredAt { get; set; }

    public OrderCreatedDomainEvent
    (
        Guid orderId,
        Guid auctionId,
        Guid sellerId,
        Guid buyerId,
        OrderStatus status,
        decimal price,
        string currency,
        DateTime paymentDeadlineUtc
    )
    {
        OrderId = orderId;
        AuctionId = auctionId;
        SellerId = sellerId;
        BuyerId = buyerId;
        Status = status;
        Price = price;
        Currency = currency;
        PaymentDeadlineUtc = paymentDeadlineUtc;
        OccurredAt = DateTime.UtcNow;
    }
}
