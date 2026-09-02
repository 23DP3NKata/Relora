using Relora.Shared.Domain.Abstractions;

namespace Relora.Orders.Domain.Events;

/// <summary>
/// Represents the order paid domain event class.
/// </summary>
public class OrderPaidDomainEvent : IDomainEvent
{
    /// <summary>
    /// Gets or sets the order id used by this type.
    /// </summary>
    public Guid OrderId { get; }
    /// <summary>
    /// Gets or sets the occurred at used by this type.
    /// </summary>
    public DateTime OccurredAt { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderPaidDomainEvent"/> class.
    /// </summary>
    /// <param name="orderId">Identifier of order.</param>
    /// <param name="paidAt">Paid at.</param>
    public OrderPaidDomainEvent(Guid orderId, DateTime paidAt)
    {
        OrderId = orderId;
        OccurredAt = paidAt;
    }
}
