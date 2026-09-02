using Relora.Shared.Domain.Abstractions;
using Relora.Shared.Domain.ValueObjects;

namespace Relora.Auctions.Domain.Events;
/// <summary>
/// Represents the bid placed domain event class.
/// </summary>
public sealed class BidPlacedDomainEvent : IDomainEvent
{
    /// <summary>
    /// Gets or sets the auction id used by this type.
    /// </summary>
    public Guid AuctionId { get; }
    /// <summary>
    /// Gets or sets the bid id used by this type.
    /// </summary>
    public Guid BidId { get; }
    /// <summary>
    /// Gets or sets the user id used by this type.
    /// </summary>
    public Guid UserId { get; }
    /// <summary>
    /// Gets or sets the money used by this type.
    /// </summary>
    public Money Money { get; } = default!;
    /// <summary>
    /// Gets or sets the occurred at used by this type.
    /// </summary>
    public DateTime OccurredAt { get; }

    public BidPlacedDomainEvent
    (
        Guid auctionId,
        Guid bidId,
        Guid userId,
        Money money,
        DateTime occeredAt
    )
    {
        AuctionId = auctionId;
        BidId = bidId;
        UserId = userId;
        Money = money;
        OccurredAt = occeredAt;
    }
}
