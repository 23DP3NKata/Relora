namespace Relora.Admin.Domain.Models;

public sealed class DeliveryIssueOrderDto
{
    public Guid OrderId { get; init; }
    public Guid AuctionId { get; init; }
    public Guid SellerId { get; init; }
    public Guid BuyerId { get; init; }
    public decimal TotalPrice { get; init; }
    public string Currency { get; init; } = default!;
    public DateTime? ShippedAtUtc { get; init; }
    public DateTime? DeliveryIssueReportedAtUtc { get; init; }
    public string? DeliveryIssueReason { get; init; }
    public string? CarrierName { get; init; }
    public string? TrackingNumber { get; init; }
}
