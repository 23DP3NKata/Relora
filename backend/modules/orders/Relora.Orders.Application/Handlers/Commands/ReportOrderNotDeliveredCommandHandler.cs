using Relora.Orders.Application.Commands;
using Relora.Orders.Application.Interfaces;
using Relora.Orders.Domain;
using Relora.Orders.Domain.Enums;
using Relora.Shared.Domain.Time;
using Relora.Shared.Domain.Persistence;
using Relora.Shared.Infrastructure.Interfaces;

using MediatR;

namespace Relora.Orders.Application.Handlers.Commands;

public sealed class ReportOrderNotDeliveredCommandHandler : IRequestHandler<ReportOrderNotDeliveredCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderDisputeRepository _disputeRepository;
    private readonly IClock _clock;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly ITransactionRunner _transactionRunner;

    public ReportOrderNotDeliveredCommandHandler(
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

    public async Task Handle(ReportOrderNotDeliveredCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            throw new KeyNotFoundException($"Order {request.OrderId} not found.");
        }

        if (order.BuyerId != request.BuyerId)
        {
            throw new UnauthorizedAccessException("Only the buyer can report a delivery issue.");
        }

        var existingOpenDispute = await _disputeRepository.GetOpenByOrderIdAsync(order.Id, cancellationToken);
        if (existingOpenDispute is not null)
        {
            throw new InvalidOperationException("This order already has an open dispute.");
        }

        var now = _clock.UtcNow;
        var dispute = OrderDispute.Open(
            order,
            request.BuyerId,
            OrderDisputeReason.ItemNotReceived,
            string.IsNullOrWhiteSpace(request.Reason)
                ? "The buyer reported that the item was not delivered."
                : request.Reason,
            [],
            now);

        await _transactionRunner.ExecuteAsync(async ct =>
        {
            await _disputeRepository.AddAsync(dispute, ct);
            await _orderRepository.UpdateOrderAsync(order, ct);
        }, cancellationToken);
        await _domainEventDispatcher.DispatchAsync(dispute.DomainEvents, cancellationToken);
        dispute.ClearDomainEvents();
        order.ClearDomainEvents();
    }
}
