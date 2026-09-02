using Relora.Shared.Domain.Abstractions;
using Relora.Shared.Domain.Enums;
using Relora.Shared.Domain.ValueObjects;

namespace Relora.Bids.Domain;

/// <summary>
/// Represents the bid class.
/// </summary>
public class Bid : Entity<Guid>
{
    /// <summary>
    /// Gets or sets the id used by this type.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the bidder id used by this type.
    /// </summary>
    public Guid BidderId { get; private set; }
    /// <summary>
    /// Gets or sets the auction id used by this type.
    /// </summary>
    public Guid AuctionId { get; private set; }
    /// <summary>
    /// Gets or sets the amount used by this type.
    /// </summary>
    public Money Amount { get; private set; }
    /// <summary>
    /// Gets or sets the placed at used by this type.
    /// </summary>
    public DateTime PlacedAt { get; private set; }
    /// <summary>
    /// Gets or sets the status used by this type.
    /// </summary>
    public BidStatus Status { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Bid"/> class.
    /// </summary>
    private Bid() { }

    public Bid(
        Guid id,
        Guid bidderId,
        Guid auctionId,
        Money amount,
        DateTime placedAt
        ) : base(id)
    {
        Id = id;
        BidderId = bidderId;
        AuctionId = auctionId;
        Amount = amount;
        PlacedAt = placedAt;
        Status = BidStatus.Active;
    }

    /// <summary>
    /// Marks as winning.
    /// </summary>
    public void MarkAsWinning()
    {
        Status = BidStatus.Winning;
    }

    /// <summary>
    /// Marks as outbid.
    /// </summary>
    public void MarkAsOutbid()
    {
        Status = BidStatus.Outbid;
    }
}

