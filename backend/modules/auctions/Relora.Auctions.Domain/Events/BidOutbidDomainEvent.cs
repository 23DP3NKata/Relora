using Relora.Shared.Domain.Abstractions;
using Relora.Shared.Domain.ValueObjects;

namespace Relora.Auctions.Domain.Events;
/// <summary>
/// Represents the bid outbid domain event class.
/// </summary>
public sealed class BidOutbidDomainEvent : IDomainEvent
{
    /// <summary>
    /// Gets or sets the auction id used by this type.
    /// </summary>
    public Guid AuctionId { get; }
    /// <summary>
    /// Gets or sets the outbid bid id used by this type.
    /// </summary>
    public Guid OutbidBidId { get; }
    /// <summary>
    /// Gets or sets the outbid user id used by this type.
    /// </summary>
    public Guid OutbidUserId { get; }
    /// <summary>
    /// Gets or sets the amount used by this type.
    /// </summary>
    public Money Amount { get; } = default!;
    /// <summary>
    /// Gets or sets the occurred at used by this type.
    /// </summary>
    public DateTime OccurredAt { get; }

    public BidOutbidDomainEvent
    (
        Guid auctionId,
        Guid outbidBidId,
        Guid outbidUserId,
        Money amount,
        DateTime occurredAt
    )
    {
        AuctionId = auctionId;
        OutbidBidId = outbidBidId;
        OutbidUserId = outbidUserId;
        Amount = amount;
        OccurredAt = occurredAt;
    }
}
