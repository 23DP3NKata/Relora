using Relora.Orders.Domain.Enums;
using Relora.Orders.Domain.Events;
using Relora.Shared.Domain.Abstractions;

namespace Relora.Orders.Domain;

public sealed class OrderDispute : AggregateRoot<Guid>
{
    private readonly List<OrderDisputeEvidence> _evidence = new();

    private OrderDispute() : base(Guid.Empty)
    {
    }

    private OrderDispute(
        Guid id,
        Guid orderId,
        Guid buyerId,
        Guid sellerId,
        OrderDisputeReason reason,
        string description,
        DateTime openedAtUtc) : base(id)
    {
        OrderId = orderId;
        BuyerId = buyerId;
        SellerId = sellerId;
        Reason = reason;
        Description = NormalizeRequired(description, "Description");
        Status = OrderDisputeStatus.Open;
        OpenedAtUtc = openedAtUtc;
    }

    public Guid OrderId { get; private set; }
    public Guid BuyerId { get; private set; }
    public Guid SellerId { get; private set; }
    public OrderDisputeReason Reason { get; private set; }
    public string Description { get; private set; } = default!;
    public OrderDisputeStatus Status { get; private set; }
    public DateTime OpenedAtUtc { get; private set; }
    public DateTime? ResolvedAtUtc { get; private set; }
    public Guid? ResolvedByAdminId { get; private set; }
    public OrderDisputeDecision? Decision { get; private set; }
    public string? DecisionReason { get; private set; }
    public IReadOnlyCollection<OrderDisputeEvidence> Evidence => _evidence.AsReadOnly();

    public static OrderDispute Open(
        Order order,
        Guid buyerId,
        OrderDisputeReason reason,
        string description,
        IReadOnlyCollection<string>? evidenceKeys,
        DateTime openedAtUtc)
    {
        if (order.BuyerId != buyerId)
        {
            throw new UnauthorizedAccessException("Only the buyer can open a dispute for this order.");
        }

        if (order.Status is
            OrderStatus.PendingPayment or
            OrderStatus.PaymentExpired or
            OrderStatus.Cancelled or
            OrderStatus.Failed or
            OrderStatus.Refunded or
            OrderStatus.Completed)
        {
            throw new InvalidOperationException("This order cannot be disputed.");
        }

        var dispute = new OrderDispute(
            Guid.NewGuid(),
            order.Id,
            order.BuyerId,
            order.SellerId,
            reason,
            description,
            openedAtUtc);

        foreach (var key in evidenceKeys ?? [])
        {
            dispute.AddEvidence(key, openedAtUtc);
        }

        order.OpenDispute(openedAtUtc);

        dispute.AddDomainEvent(new OrderDisputeOpenedDomainEvent(
            dispute.Id,
            order.Id,
            order.BuyerId,
            order.SellerId,
            reason.ToString(),
            openedAtUtc));

        return dispute;
    }

    public void Resolve(
        Order order,
        Guid adminId,
        OrderDisputeDecision decision,
        string decisionReason,
        DateTime resolvedAtUtc)
    {
        if (Status != OrderDisputeStatus.Open)
        {
            throw new InvalidOperationException("Only open disputes can be resolved.");
        }

        if (order.Id != OrderId)
        {
            throw new InvalidOperationException("Dispute does not belong to this order.");
        }

        Status = OrderDisputeStatus.Resolved;
        Decision = decision;
        DecisionReason = NormalizeRequired(decisionReason, "Decision reason");
        ResolvedAtUtc = resolvedAtUtc;
        ResolvedByAdminId = adminId;

        if (decision == OrderDisputeDecision.Refund)
        {
            order.MarkAsRefunded(resolvedAtUtc);
        }
        else
        {
            order.CompleteAfterDispute(resolvedAtUtc);
        }

        AddDomainEvent(new OrderDisputeResolvedDomainEvent(
            Id,
            OrderId,
            BuyerId,
            SellerId,
            decision.ToString(),
            DecisionReason,
            resolvedAtUtc));
    }

    private void AddEvidence(string key, DateTime createdAtUtc)
    {
        if (_evidence.Any(x => x.Key == key.Trim()))
        {
            return;
        }

        _evidence.Add(new OrderDisputeEvidence(Id, key, createdAtUtc));
    }

    private static string NormalizeRequired(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{fieldName} is required.");
        }

        return value.Trim();
    }
}
