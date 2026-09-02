using Relora.Shared.Domain.Abstractions;
using Relora.Shared.Domain.ValueObjects;

using MediatR;

namespace Relora.Auctions.Domain.Events;

/// <summary>
/// Represents the auction ended domain event class.
/// </summary>
public sealed class AuctionEndedDomainEvent : IDomainEvent
{
    /// <summary>
    /// Gets or sets the auction id used by this type.
    /// </summary>
    public Guid AuctionId { get; }
    /// <summary>
    /// Gets or sets the lot id used by this type.
    /// </summary>
    public Guid LotId { get; }
    /// <summary>
    /// Gets or sets the winner id used by this type.
    /// </summary>
    public Guid? WinnerId { get; }
    /// <summary>
    /// Gets or sets the winning bid id used by this type.
    /// </summary>
    public Guid? WinningBidId { get; }
    /// <summary>
    /// Gets or sets the winning money used by this type.
    /// </summary>
    public Money? WinningMoney { get; }
    /// <summary>
    /// Gets or sets the occurred at used by this type.
    /// </summary>
    public DateTime OccurredAt { get; }

    public AuctionEndedDomainEvent(
        Guid auctionId,
        Guid lotId,
        Guid? winnerId,
        Guid? winningBidId,
        Money? winningMoney,
        DateTime occurredAt)
    {
        AuctionId = auctionId;
        LotId = lotId;
        WinnerId = winnerId;
        WinningBidId = winningBidId;
        WinningMoney = winningMoney;
        OccurredAt = occurredAt;
    }
}
