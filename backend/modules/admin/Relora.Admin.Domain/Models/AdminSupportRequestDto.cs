namespace Relora.Admin.Domain.Models;

public sealed class AdminSupportRequestDto
{
    public Guid Id { get; init; }
    public Guid? UserId { get; init; }
    public string? Username { get; init; }
    public string Email { get; init; } = default!;
    public string Category { get; init; } = default!;
    public string Subject { get; init; } = default!;
    public string Message { get; init; } = default!;
    public string StatusName { get; init; } = default!;
    public DateTime CreatedAt { get; init; }
}
