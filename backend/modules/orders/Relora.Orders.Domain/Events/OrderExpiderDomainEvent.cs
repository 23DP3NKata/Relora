using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Relora.Shared.Domain.Abstractions;

namespace Relora.Orders.Domain.Events;
/// <summary>
/// Represents the order expider domain event class.
/// </summary>
public sealed class OrderExpiderDomainEvent : IDomainEvent
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
    /// Initializes a new instance of the <see cref="OrderExpiderDomainEvent"/> class.
    /// </summary>
    /// <param name="orderId">Identifier of order.</param>
    /// <param name="expiredAt">Expired at.</param>
    public OrderExpiderDomainEvent(Guid orderId, DateTime expiredAt)
    {
        OrderId = orderId;
        OccurredAt = expiredAt;
    }
}
