using Relora.Orders.Domain.Enums;

namespace Relora.Orders.Application.Models;

public sealed class OrderDisputeDto
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public OrderDisputeReason Reason { get; init; }
    public string ReasonName { get; init; } = default!;
    public string Description { get; init; } = default!;
    public OrderDisputeStatus Status { get; init; }
    public string StatusName { get; init; } = default!;
    public DateTime OpenedAtUtc { get; init; }
    public DateTime? ResolvedAtUtc { get; init; }
    public Guid? ResolvedByAdminId { get; init; }
    public OrderDisputeDecision? Decision { get; init; }
    public string? DecisionName { get; init; }
    public string? DecisionReason { get; init; }
    public IReadOnlyList<OrderDisputeEvidenceDto> Evidence { get; init; } = [];
}
