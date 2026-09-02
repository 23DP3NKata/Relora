using Relora.Shared.Domain.Abstractions;

namespace Relora.Orders.Domain.Events;

public sealed class OrderDeliveryIssueReportedDomainEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public Guid BuyerId { get; }
    public Guid SellerId { get; }
    public string? Reason { get; }
    public DateTime OccurredAt { get; }

    public OrderDeliveryIssueReportedDomainEvent(
        Guid orderId,
        Guid buyerId,
        Guid sellerId,
        string? reason,
        DateTime occurredAt)
    {
        OrderId = orderId;
        BuyerId = buyerId;
        SellerId = sellerId;
        Reason = reason;
        OccurredAt = occurredAt;
    }
}
