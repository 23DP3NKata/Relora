namespace Relora.Admin.Domain.Models;

public sealed class OverdueShipmentOrderDto
{
    public Guid OrderId { get; init; }
    public Guid AuctionId { get; init; }
    public Guid SellerId { get; init; }
    public Guid BuyerId { get; init; }
    public decimal TotalPrice { get; init; }
    public string Currency { get; init; } = default!;
    public DateTime? PaidAtUtc { get; init; }
    public DateTime? ShipByUtc { get; init; }
    public int DaysOverdue { get; init; }
}
