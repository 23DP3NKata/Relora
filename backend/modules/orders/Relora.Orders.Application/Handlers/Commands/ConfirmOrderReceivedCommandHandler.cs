using Relora.Orders.Application.Commands;
using Relora.Orders.Application.Interfaces;
using Relora.Shared.Domain.Time;
using Relora.Shared.Infrastructure.Interfaces;

using MediatR;

namespace Relora.Orders.Application.Handlers.Commands;

public sealed class ConfirmOrderReceivedCommandHandler : IRequestHandler<ConfirmOrderReceivedCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IClock _clock;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public ConfirmOrderReceivedCommandHandler(
        IOrderRepository orderRepository,
        IClock clock,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _orderRepository = orderRepository;
        _clock = clock;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task Handle(ConfirmOrderReceivedCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            throw new KeyNotFoundException($"Order {request.OrderId} not found.");
        }

        if (order.BuyerId != request.BuyerId)
        {
            throw new UnauthorizedAccessException("Only the buyer can confirm receipt.");
        }

        order.ConfirmReceived(_clock.UtcNow);

        await _orderRepository.UpdateOrderAsync(order, cancellationToken);
        await _domainEventDispatcher.DispatchAsync(order.DomainEvents, cancellationToken);
        order.ClearDomainEvents();
    }
}
