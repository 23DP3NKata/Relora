using Relora.Shared.Domain.Enums;

namespace Relora.Identity.Application.Models;

public sealed class UserProfileListingDto
{
    public Guid LotId { get; set; }
    public string Title { get; set; } = default!;
    public string? Brand { get; set; }
    public string? ThumbnailUrl { get; set; }

    public decimal CurrentPrice { get; set; }
    public string Currency { get; set; } = default!;

    public LotStatus Status { get; set; } = default!;
}
