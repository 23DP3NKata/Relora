using Relora.Shared.Domain.Abstractions;

namespace Relora.Orders.Domain.Events;

public sealed class OrderShippedDomainEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public Guid BuyerId { get; }
    public string CarrierName { get; }
    public string TrackingNumber { get; }
    public DateTime OccurredAt { get; }

    public OrderShippedDomainEvent(
        Guid orderId,
        Guid buyerId,
        string carrierName,
        string trackingNumber,
        DateTime occurredAt)
    {
        OrderId = orderId;
        BuyerId = buyerId;
        CarrierName = carrierName;
        TrackingNumber = trackingNumber;
        OccurredAt = occurredAt;
    }
}
