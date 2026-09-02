using Relora.Shared.Domain.Abstractions;

namespace Relora.Orders.Domain.Events;

public sealed class OrderDisputeOpenedDomainEvent : IDomainEvent
{
    public Guid DisputeId { get; }
    public Guid OrderId { get; }
    public Guid BuyerId { get; }
    public Guid SellerId { get; }
    public string Reason { get; }
    public DateTime OccurredAt { get; }

    public OrderDisputeOpenedDomainEvent(
        Guid disputeId,
        Guid orderId,
        Guid buyerId,
        Guid sellerId,
        string reason,
        DateTime occurredAt)
    {
        DisputeId = disputeId;
        OrderId = orderId;
        BuyerId = buyerId;
        SellerId = sellerId;
        Reason = reason;
        OccurredAt = occurredAt;
    }
}
