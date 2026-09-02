using Relora.Identity.Application.Interfaces;
using Relora.Orders.Application.Commands;
using Relora.Orders.Application.Interfaces;
using Relora.Shared.Domain.Time;
using Relora.Shared.Domain.Persistence;
using Relora.Shared.Infrastructure.Interfaces;
using Relora.Shared.Application.Payments;
using Relora.Orders.Domain.Enums;

using MediatR;

namespace Relora.Orders.Application.Handlers.Commands;

public sealed class ResolveOrderDisputeCommandHandler : IRequestHandler<ResolveOrderDisputeCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderDisputeRepository _disputeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IClock _clock;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly IPaymentRefundService _paymentRefundService;
    private readonly ITransactionRunner _transactionRunner;

    public ResolveOrderDisputeCommandHandler(
        IOrderRepository orderRepository,
        IOrderDisputeRepository disputeRepository,
        IUserRepository userRepository,
        IClock clock,
        IDomainEventDispatcher domainEventDispatcher,
        IPaymentRefundService paymentRefundService,
        ITransactionRunner transactionRunner)
    {
        _orderRepository = orderRepository;
        _disputeRepository = disputeRepository;
        _userRepository = userRepository;
        _clock = clock;
        _domainEventDispatcher = domainEventDispatcher;
        _paymentRefundService = paymentRefundService;
        _transactionRunner = transactionRunner;
    }

    public async Task Handle(ResolveOrderDisputeCommand request, CancellationToken cancellationToken)
    {
        var admin = await _userRepository.GetUserByIdAsync(request.AdminId);
        if (admin is null || !admin.IsAdmin)
        {
            throw new UnauthorizedAccessException("User is not an admin.");
        }

        var dispute = await _disputeRepository.GetByIdAsync(request.DisputeId, cancellationToken);
        if (dispute is null)
        {
            throw new KeyNotFoundException($"Dispute {request.DisputeId} not found.");
        }

        var order = await _orderRepository.GetOrderByIdAsync(dispute.OrderId, cancellationToken);
        if (order is null)
        {
            throw new KeyNotFoundException($"Order {dispute.OrderId} not found.");
        }

        if (request.Decision == OrderDisputeDecision.Refund)
        {
            await _paymentRefundService.RefundOrderAsync(order.Id, cancellationToken);
        }

        dispute.Resolve(order, request.AdminId, request.Decision, request.Reason, _clock.UtcNow);

        await _transactionRunner.ExecuteAsync(async ct =>
        {
            await _disputeRepository.UpdateAsync(dispute, ct);
            await _orderRepository.UpdateOrderAsync(order, ct);
        }, cancellationToken);
        await _domainEventDispatcher.DispatchAsync(dispute.DomainEvents.Concat(order.DomainEvents), cancellationToken);
        dispute.ClearDomainEvents();
        order.ClearDomainEvents();
    }
}
