using Relora.Orders.Application.Commands;
using Relora.Orders.Application.Interfaces;
using Relora.Orders.Domain.Enums;
using Relora.Orders.Infrastructure.Options;
using Relora.Shared.Domain.Time;
using Relora.Shared.Domain.Persistence;
using Relora.Payments.Application.Interfaces;

using MediatR;

using Stripe.Checkout;

namespace Relora.Payments.Application.Service;

/// <summary>
/// Represents the payment service class.
/// </summary>
public sealed class PaymentService 
{
    private readonly IOrderRepository _orders;
    private readonly IPaymentRepository _payments;
    private readonly IClock _clock;
    private readonly IMediator _mediator;
    private readonly ITransactionRunner _transactions;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentService"/> class.
    /// </summary>
    /// <param name="orders">Orders.</param>
    /// <param name="clock">Clock.</param>
    public PaymentService(
        IOrderRepository orders,
        IPaymentRepository payments,
        IClock clock,
        IMediator mediator,
        ITransactionRunner transactions)
    {
        _orders = orders;
        _payments = payments;
        _clock = clock;
        _mediator = mediator;
        _transactions = transactions;
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

        var payment = await _payments.GetByStripeSessionIdAsync(session.Id, cancellationToken);

        order.ClearStripeSession(session.Id);
        if (payment is not null)
        {
            payment.MarkCancelled();
        }

        await _transactions.ExecuteAsync(async ct =>
        {
            await _orders.UpdateOrderAsync(order, ct);

            if (payment is not null)
            {
                await _payments.UpdatePaymentAsync(payment, ct);
            }
        }, cancellationToken);
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

        if (!string.Equals(order.StripeCheckoutSessionId, session.Id, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Stripe session does not match order.");
        }

        var payment = await _payments.GetByStripeSessionIdAsync(session.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Payment for Stripe session {session.Id} was not found.");

        if (order.Status is OrderStatus.Paid or OrderStatus.AwaitingShipment or OrderStatus.Shipped or OrderStatus.Completed)
        {
            if (payment.Status == Relora.Payments.Domain.Enums.PaymentStatus.Pending)
            {
                payment.MarkPaid(RequirePaymentIntentId(session), _clock.UtcNow);
                await _transactions.ExecuteAsync(
                    ct => _payments.UpdatePaymentAsync(payment, ct),
                    cancellationToken);
            }

            return;
        }

        if (order.Status != OrderStatus.PendingPayment)
        {
            throw new InvalidOperationException("Order is not pending payment.");
        }

        var now = _clock.UtcNow;
        order.MarkAsPaid(now);
        payment.MarkPaid(RequirePaymentIntentId(session), now);

        await _transactions.ExecuteAsync(async ct =>
        {
            await _orders.UpdateOrderAsync(order, ct);
            await _payments.UpdatePaymentAsync(payment, ct);
        }, cancellationToken);
        await PublishOrderEvents(order, cancellationToken);
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

        var payment = await _payments.GetByStripeSessionIdAsync(session.Id, cancellationToken);
        order.ClearStripeSession(session.Id);

        if (payment is not null)
        {
            payment.MarkFailed();
        }

        await _transactions.ExecuteAsync(async ct =>
        {
            await _orders.UpdateOrderAsync(order, ct);

            if (payment is not null)
            {
                await _payments.UpdatePaymentAsync(payment, ct);
            }
        }, cancellationToken);
    }

    private static string RequirePaymentIntentId(Session session)
    {
        if (string.IsNullOrWhiteSpace(session.PaymentIntentId))
        {
            throw new InvalidOperationException("Stripe checkout session has no payment intent.");
        }

        return session.PaymentIntentId;
    }

    private async Task PublishOrderEvents(Relora.Orders.Domain.Order order, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in order.DomainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        order.ClearDomainEvents();
    }


}
