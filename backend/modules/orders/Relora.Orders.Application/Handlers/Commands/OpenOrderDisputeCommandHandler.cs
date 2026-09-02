using Relora.Orders.Application.Commands;
using Relora.Orders.Application.Interfaces;
using Relora.Orders.Domain;
using Relora.Shared.Domain.Time;
using Relora.Shared.Domain.Persistence;
using Relora.Shared.Infrastructure.Interfaces;

using MediatR;

namespace Relora.Orders.Application.Handlers.Commands;

public sealed class OpenOrderDisputeCommandHandler : IRequestHandler<OpenOrderDisputeCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderDisputeRepository _disputeRepository;
    private readonly IClock _clock;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly ITransactionRunner _transactionRunner;

    public OpenOrderDisputeCommandHandler(
        IOrderRepository orderRepository,
        IOrderDisputeRepository disputeRepository,
        IClock clock,
        IDomainEventDispatcher domainEventDispatcher,
        ITransactionRunner transactionRunner)
    {
        _orderRepository = orderRepository;
        _disputeRepository = disputeRepository;
        _clock = clock;
        _domainEventDispatcher = domainEventDispatcher;
        _transactionRunner = transactionRunner;
    }

    public async Task<Guid> Handle(OpenOrderDisputeCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            throw new KeyNotFoundException($"Order {request.OrderId} not found.");
        }

        var existingOpenDispute = await _disputeRepository.GetOpenByOrderIdAsync(order.Id, cancellationToken);
        if (existingOpenDispute is not null)
        {
            throw new InvalidOperationException("This order already has an open dispute.");
        }

        var dispute = OrderDispute.Open(
            order,
            request.BuyerId,
            request.Reason,
            request.Description,
            request.EvidenceKeys,
            _clock.UtcNow);

        await _transactionRunner.ExecuteAsync(async ct =>
        {
            await _disputeRepository.AddAsync(dispute, ct);
            await _orderRepository.UpdateOrderAsync(order, ct);
        }, cancellationToken);
        await _domainEventDispatcher.DispatchAsync(dispute.DomainEvents, cancellationToken);
        dispute.ClearDomainEvents();

        return dispute.Id;
    }
}
