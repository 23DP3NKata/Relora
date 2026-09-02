using Relora.Shared.Domain.Abstractions;

namespace Relora.Auctions.Domain.Events;

/// <summary>
/// Represents the auction started domain event class.
/// </summary>
public sealed class AuctionStartedDomainEvent : IDomainEvent
{
    /// <summary>
    /// Gets the auction id.
    /// </summary>
    public Guid AuctionId { get; }

    /// <summary>
    /// Gets the lot id.
    /// </summary>
    public Guid? LotId { get; }

    /// <summary>
    /// Gets the start date.
    /// </summary>
    public DateTime StartDate { get; }

    /// <summary>
    /// Gets the end date.
    /// </summary>
    public DateTime EndDate { get; }

    /// <summary>
    /// Gets the occurred at date.
    /// </summary>
    public DateTime OccurredAt { get; }

    public AuctionStartedDomainEvent(
        Guid auctionId,
        Guid? lotId,
        DateTime startDate,
        DateTime endDate,
        DateTime occurredAt)
    {
        AuctionId = auctionId;
        LotId = lotId;
        StartDate = startDate;
        EndDate = endDate;
        OccurredAt = occurredAt;
    }
}
