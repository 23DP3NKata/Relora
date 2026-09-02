using Relora.Orders.Application.Commands;
using Relora.Orders.Application.Interfaces;
using Relora.Shared.Domain.Time;
using Relora.Shared.Infrastructure.Interfaces;

using MediatR;

namespace Relora.Orders.Application.Handlers.Commands;

public sealed class MarkOrderShippedCommandHandler : IRequestHandler<MarkOrderShippedCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IClock _clock;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public MarkOrderShippedCommandHandler(
        IOrderRepository orderRepository,
        IClock clock,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _orderRepository = orderRepository;
        _clock = clock;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task Handle(MarkOrderShippedCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            throw new KeyNotFoundException($"Order {request.OrderId} not found.");
        }

        if (order.SellerId != request.SellerId)
        {
            throw new UnauthorizedAccessException("Only the seller can add shipment details.");
        }

        order.MarkAsShipped(request.CarrierName, request.TrackingNumber, _clock.UtcNow);

        await _orderRepository.UpdateOrderAsync(order, cancellationToken);
        await _domainEventDispatcher.DispatchAsync(order.DomainEvents, cancellationToken);
        order.ClearDomainEvents();
    }
}
