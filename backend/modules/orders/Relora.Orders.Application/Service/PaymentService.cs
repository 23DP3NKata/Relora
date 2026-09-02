using Relora.Orders.Application.Commands;
using Relora.Orders.Application.Interfaces;
using Relora.Orders.Domain.Enums;
using Relora.Orders.Infrastructure.Options;
using Relora.Shared.Domain.Time;

using MediatR;

using Stripe.Checkout;

namespace Relora.Orders.Application.Service;

/// <summary>
/// Represents the payment service class.
/// </summary>
public sealed class PaymentService 
{
    private readonly IOrderRepository _orders;
    private readonly IClock _clock;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentService"/> class.
    /// </summary>
    /// <param name="orders">Orders.</param>
    /// <param name="clock">Clock.</param>
    public PaymentService(IOrderRepository orders, IClock clock)
    {
        _orders = orders;
        _clock = clock;
    }

    /// <summary>
    /// Handles expired checkout session.
    /// </summary>
    /// <param name="session">Session.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task HandleExpiredCheckoutSession(Session session, CancellationToken cancellationToken)
    {
        var orderIdRaw = session.Metadata.GetValueOrDefault("orderId") ?? session.ClientReferenceId;

        if (!Guid.TryParse(orderIdRaw, out var orderId))
        {
            throw new InvalidOperationException("Invalid order ID in session metadata.");
        }

        var order = await _orders.GetOrderByIdAsync(orderId, cancellationToken);

        if (order == null)
        {
            throw new KeyNotFoundException($"Order {orderId} not found.");
        }

        if (order.Status != OrderStatus.PendingPayment)
        {
            return;
        }

        order.ClearStripeSession(session.Id);
        await _orders.UpdateOrderAsync(order, cancellationToken);
    }

    /// <summary>
    /// Handles successful checkout session.
    /// </summary>
    /// <param name="session">Session.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task HandleSuccessfulCheckoutSession(Session session, CancellationToken cancellationToken)
    {
        var orderIdRaw = session.Metadata.GetValueOrDefault("orderId") ?? session.ClientReferenceId;

        if (!Guid.TryParse(orderIdRaw, out var orderId))
        {
            throw new InvalidOperationException("Invalid order ID in session metadata.");
        }

        var order = await _orders.GetOrderByIdAsync(orderId, cancellationToken);

        if (order == null)
        {
            throw new KeyNotFoundException($"Order {orderId} not found.");
        }

        if (order.Status is OrderStatus.Paid or OrderStatus.AwaitingShipment or OrderStatus.Shipped or OrderStatus.Completed)
        {
            return;
        }

        if (order.Status != OrderStatus.PendingPayment)
        {
            throw new InvalidOperationException("Order is not pending payment.");
        }

        if (!string.Equals(order.StripeCheckoutSessionId, session.Id, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Stripe session does not match order.");
        }

        var now = _clock.UtcNow;
        order.MarkAsPaid(now);
        await _orders.UpdateOrderAsync(order, cancellationToken);
    }

    /// <summary>
    /// Handles failed checkout session.
    /// </summary>
    /// <param name="session">Session.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task HandleFailedCheckoutSession(Session session, CancellationToken cancellationToken)
    {
        var orderIdRaw = session.Metadata.GetValueOrDefault("orderId") ?? session.ClientReferenceId;

        if (!Guid.TryParse(orderIdRaw, out var orderId))
        {
            throw new InvalidOperationException("Invalid order ID in session metadata.");
        }

        var order = await _orders.GetOrderByIdAsync(orderId, cancellationToken);

        if (order == null)
        {
            throw new KeyNotFoundException($"Order {orderId} not found.");
        }

        if (order.Status != OrderStatus.PendingPayment)
        {
            return;
        }

        if (!string.Equals(order.StripeCheckoutSessionId, session.Id, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Stripe session does not match order.");
        }

        order.ClearStripeSession(session.Id);
        await _orders.UpdateOrderAsync(order, cancellationToken);
    }


}
