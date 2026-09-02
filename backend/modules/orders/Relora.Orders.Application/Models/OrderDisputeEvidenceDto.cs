namespace Relora.Orders.Application.Models;

public sealed class OrderDisputeEvidenceDto
{
    public Guid Id { get; init; }
    public string Key { get; init; } = default!;
    public DateTime CreatedAtUtc { get; init; }
}
