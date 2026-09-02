namespace Relora.Admin.Domain.Models;

public sealed class AdminOrderDisputeDto
{
    public Guid DisputeId { get; init; }
    public Guid OrderId { get; init; }
    public Guid AuctionId { get; init; }
    public Guid? LotId { get; init; }
    public string? LotTitle { get; init; }
    public string? LotBrand { get; init; }
    public string? LotPhotoKey { get; init; }
    public Guid SellerId { get; init; }
    public Guid BuyerId { get; init; }
    public decimal TotalPrice { get; init; }
    public string Currency { get; init; } = default!;
    public string OrderStatusName { get; init; } = default!;
    public string ReasonName { get; init; } = default!;
    public string Description { get; init; } = default!;
    public string DisputeStatusName { get; init; } = default!;
    public DateTime OpenedAtUtc { get; init; }
    public DateTime? ResolvedAtUtc { get; init; }
    public Guid? ResolvedByAdminId { get; init; }
    public string? DecisionName { get; init; }
    public string? DecisionReason { get; init; }
    public bool PayoutBlocked { get; init; }
    public DateTime? ShippedAtUtc { get; init; }
    public string? CarrierName { get; init; }
    public string? TrackingNumber { get; init; }
    public IReadOnlyList<AdminOrderDisputeEvidenceDto> Evidence { get; init; } = [];
}
