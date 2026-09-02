namespace Relora.Admin.Domain.Models;

public sealed class AdminOrderDisputeEvidenceDto
{
    public Guid Id { get; init; }
    public string Key { get; init; } = default!;
    public DateTime CreatedAtUtc { get; init; }
}
