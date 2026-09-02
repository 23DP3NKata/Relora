using Relora.Shared.Domain.Abstractions;

namespace Relora.Orders.Domain.Events;

public sealed class OrderDisputeResolvedDomainEvent : IDomainEvent
{
    public Guid DisputeId { get; }
    public Guid OrderId { get; }
    public Guid BuyerId { get; }
    public Guid SellerId { get; }
    public string Decision { get; }
    public string DecisionReason { get; }
    public DateTime OccurredAt { get; }

    public OrderDisputeResolvedDomainEvent(
        Guid disputeId,
        Guid orderId,
        Guid buyerId,
        Guid sellerId,
        string decision,
        string decisionReason,
        DateTime occurredAt)
    {
        DisputeId = disputeId;
        OrderId = orderId;
        BuyerId = buyerId;
        SellerId = sellerId;
        Decision = decision;
        DecisionReason = decisionReason;
        OccurredAt = occurredAt;
    }
}
