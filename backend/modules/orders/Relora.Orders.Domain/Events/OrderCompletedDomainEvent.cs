using Relora.Shared.Domain.Abstractions;

namespace Relora.Orders.Domain.Events;

public sealed class OrderCompletedDomainEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public Guid BuyerId { get; }
    public Guid SellerId { get; }
    public DateTime OccurredAt { get; }

    public OrderCompletedDomainEvent(
        Guid orderId,
        Guid buyerId,
        Guid sellerId,
        DateTime occurredAt)
    {
        OrderId = orderId;
        BuyerId = buyerId;
        SellerId = sellerId;
        OccurredAt = occurredAt;
    }
}
