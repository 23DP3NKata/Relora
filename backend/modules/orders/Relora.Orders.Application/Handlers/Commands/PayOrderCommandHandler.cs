using Relora.Orders.Application.Commands;
using Relora.Orders.Application.Interfaces;
using Relora.Shared.Domain.Time;
using Relora.Shared.Infrastructure.Interfaces;

using MediatR;

namespace Relora.Orders.Application.Handlers.Commands;

public sealed class PayOrderCommandHandler : IRequestHandler<PayOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IClock _clock;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public PayOrderCommandHandler(
        IOrderRepository orderRepository,
        IClock clock,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _orderRepository = orderRepository;
        _clock = clock;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task Handle(PayOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            throw new KeyNotFoundException($"Order {request.OrderId} not found.");
        }

        if (order.BuyerId != request.BuyerId)
        {
            throw new UnauthorizedAccessException("Only the buyer can pay for this order.");
        }

        // address can be sent together with the payment
        if (request.ShippingAddress is not null)
        {
            order.SetShippingAddress(
                request.ShippingAddress.FullName,
                request.ShippingAddress.CountryCode,
                request.ShippingAddress.Country,
                request.ShippingAddress.City,
                request.ShippingAddress.PostalCode,
                request.ShippingAddress.AddressLine1,
                request.ShippingAddress.AddressLine2,
                request.ShippingAddress.Phone);
        }

        if (!order.HasShippingAddress)
        {
            throw new InvalidOperationException("Shipping address is required before payment.");
        }

        // checks status and deadline, then moves order to awaiting shipment
        order.MarkAsPaid(_clock.UtcNow);

        await _orderRepository.UpdateOrderAsync(order, cancellationToken);
        await _domainEventDispatcher.DispatchAsync(order.DomainEvents, cancellationToken);
        order.ClearDomainEvents();
    }
}
