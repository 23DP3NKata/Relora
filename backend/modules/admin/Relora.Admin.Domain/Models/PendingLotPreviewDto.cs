using Relora.Items.Domain.Enums;
using Relora.Shared.Domain.Enums;
using Relora.Shared.Domain.ValueObjects;

namespace Relora.Admin.Domain.Models;

public sealed class PendingLotPreviewDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Brand { get; init; } = string.Empty;

    public decimal PriceAmount { get; init; }
    public string Currency { get; init; }

    public LotCondition Condition { get; init; }
    public LotStatus Status { get; init; }

    public Guid SellerId { get; init; }
    public string? MainPhotoKey { get; init; }
    public DateTime CreatedAt { get; init; }
}
