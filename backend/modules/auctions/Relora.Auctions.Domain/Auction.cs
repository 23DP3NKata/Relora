using Relora.Shared.Domain.Abstractions;
using Relora.Shared.Domain.Time;
using Relora.Shared.Domain.ValueObjects;
using Relora.Shared.Domain.Enums;
using Relora.Bids.Domain;
using Relora.Items.Domain;
using Relora.Auctions.Domain.Events;
using System.Threading.Tasks;

namespace Relora.Auctions.Domain;
/// <summary>
/// Represents the auction class.
/// </summary>
public sealed class Auction : AggregateRoot<Guid>
{
    /// <summary>
    /// Gets or sets the status used by this type.
    /// </summary>
    public AuctionStatus Status { get; private set; }
    /// <summary>
    /// Gets or sets the start date used by this type.
    /// </summary>
    public DateTime? StartDate { get; private set; }
    /// <summary>
    /// Gets or sets the end date used by this type.
    /// </summary>
    public DateTime? EndDate { get; private set; }
    /// <summary>
    /// Gets or sets the current price used by this type.
    /// </summary>
    public Money CurrentPrice { get; private set; }
    /// <summary>
    /// Gets or sets the lot id used by this type.
    /// </summary>
    public Guid? LotId { get; private set; }

    private readonly List<Bid> _bids = new();
    public IReadOnlyCollection<Bid> Bids => _bids.AsReadOnly();

    private static readonly TimeSpan AntiSnipingWindow = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan AntiSnipingExtension = TimeSpan.FromSeconds(30);

    private Auction() : base(Guid.Empty)
    {
    }

    public Auction(Guid id, Money currentPrice) : base(id)
    {
        Status = AuctionStatus.Draft;
        StartDate = DateTime.UtcNow;
        EndDate = DateTime.UtcNow.AddDays(7);
        CurrentPrice = currentPrice;
    }

    /// <summary>
    /// Starts auction.
    /// </summary>
    /// <param name="now">Date and time for the operation.</param>
    /// <param name="duration">Duration.</param>
    public void StartAuction(DateTime now, TimeSpan duration)
    {
        if (Status != AuctionStatus.Draft && Status != AuctionStatus.Unsold)
        {
            throw new InvalidOperationException("Only draft or unsold auctions can be started.");
        }

        if (duration <= TimeSpan.Zero)
        {
            throw new ArgumentException("Duration must be positive.");
        }

        if (duration > TimeSpan.FromDays(30))
        {
            throw new ArgumentException("Auction must be no longer than 30 days.");
        }

        StartDate = now;
        EndDate = now.Add(duration);
        Status = AuctionStatus.Active;

        AddDomainEvent(new AuctionStartedDomainEvent(
            auctionId: Id,
            lotId: LotId,
            startDate: StartDate.Value,
            endDate: EndDate.Value,
            occurredAt: now
        ));
    }

    /// <summary>
    /// Stops auction.
    /// </summary>
    /// <param name="now">Date and time for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task StopAuction(DateTime now)
    {
        if (Status != AuctionStatus.Active)
        {
            throw new InvalidOperationException("Only active auctions can be stopped.");
        }

        if (EndDate == null || now < EndDate.Value)
        {
            throw new InvalidOperationException("Auction has not ended yet.");
        }

        Bid? winningBid = null;

        if (_bids.Any())
        {
            winningBid = _bids
                .OrderByDescending(b => b.Amount.Amount)
                .First();

            foreach (var bid in _bids)
            {
                if (bid == winningBid)
                {
                    bid.MarkAsWinning();
                }
                else
                {
                    bid.MarkAsOutbid();
                }
            }
        }

        Status = winningBid is null
            ? AuctionStatus.Unsold
            : AuctionStatus.Finished;
        EndDate = now;

        AddDomainEvent(new AuctionEndedDomainEvent(
            auctionId: Id,
            lotId: LotId ?? Guid.Empty,
            winnerId: winningBid?.BidderId,
            winningBidId: winningBid?.Id,
            winningMoney: winningBid?.Amount,
            occurredAt: now
        ));
    }

    /// <summary>
    /// Cancels auction.
    /// </summary>
    public void CancelAuction()
    {
        if (Status == AuctionStatus.Finished)
        {
            throw new InvalidOperationException("Finished auctions cannot be cancelled.");
        }

        Status = AuctionStatus.Cancelled;
    }

    /// <summary>
    /// Reschedules auction.
    /// </summary>
    /// <param name="newStartDate">Date and time for the operation.</param>
    /// <param name="newDuration">New duration.</param>
    public void RescheduleAuction(DateTime newStartDate, TimeSpan newDuration)
    {
        if (Status != AuctionStatus.Draft)
        {
            throw new InvalidOperationException("Only draft auctions can be rescheduled.");
        }

        if (newDuration <= TimeSpan.Zero)
        {
            throw new ArgumentException("Duration must be positive.");
        }

        if (newDuration > TimeSpan.FromDays(30))
        {
            throw new ArgumentException("Auction must be no longer than 30 days.");
        }

        StartDate = newStartDate;
        EndDate = newStartDate.Add(newDuration);
    }

    /// <summary>
    /// Places bid.
    /// </summary>
    /// <param name="bidId">Identifier of bid.</param>
    /// <param name="userId">Identifier of user.</param>
    /// <param name="amount">Amount.</param>
    /// <param name="now">Date and time for the operation.</param>
    public void PlaceBid(Guid bidId, Guid userId, Money amount, DateTime now)
    {
        if (Status != AuctionStatus.Active)
        {
            throw new InvalidOperationException("Bids can only be placed on active auctions.");
        }

        if (EndDate == null || now >= EndDate)
        {
            throw new InvalidOperationException("Cannot place bid on ended auction.");
        }

        if (CurrentPrice != null)
        {
            if (!amount.GreatherThan(CurrentPrice))
            {
                throw new InvalidOperationException("Bid must be higher than current price.");
            }

            if (amount.Currency != CurrentPrice.Currency)
            {
                throw new InvalidOperationException("Bid currency must match current price currency.");
            }
        }

        var lastBid = _bids
            .Where(b => b.Status == BidStatus.Active)
            .OrderByDescending(b => b.PlacedAt)
            .FirstOrDefault();

        if (lastBid != null)
        {
            lastBid.MarkAsOutbid();
        }

        var bidAmount = new Money(amount.Amount, amount.Currency);
        var currentPriceAmount = new Money(amount.Amount, amount.Currency);

        var bid = new Bid(
            bidId,
            userId,
            Id,
            bidAmount,
            now
        );

        _bids.Add(bid);

        CurrentPrice = currentPriceAmount;

        AntiSnipping(now);

        AddDomainEvent(new BidPlacedDomainEvent(Id, bidId, userId, bidAmount, now));
    }

    /// <summary>
    /// Adds lot.
    /// </summary>
    /// <param name="lot">Lot.</param>
    public void AddLot(Lot lot)
    {
        if (lot == null)
        {
            throw new ArgumentNullException("Lot cannot be null.");
        }

        if (Status != AuctionStatus.Draft)
        {
            throw new InvalidOperationException("Cannot add lots to a started auction.");
        }

        if (lot.Status != LotStatus.Published && lot.Status != LotStatus.Unsold)
        {
            throw new InvalidOperationException("Only published or unsold lots can be added to auction.");
        }

        if (lot.Id == LotId)
        {
            throw new InvalidOperationException("Lot is already added to this auction.");
        }

        LotId = lot.Id;
    }

    /// <summary>
    /// Removes lot.
    /// </summary>
    /// <param name="lot">Lot.</param>
    private void RemoveLot(Lot lot)
    {
        if (Status != AuctionStatus.Draft)
        {
            throw new InvalidOperationException("Cannot remove lots from a started auction.");
        }

        if (LotId != lot.Id)
        {
            throw new InvalidOperationException("Lot is not part of this auction.");
        }

        LotId = Guid.Empty;
    }

    /// <summary>
    /// Adds bid.
    /// </summary>
    /// <param name="bid">Identifier of b.</param>
    private void AddBid(Bid bid)
    {
        if (bid == null)
        {
            throw new ArgumentNullException("Bid cannot be null.");
        }

        if (Status != AuctionStatus.Active)
        {
            throw new InvalidOperationException("Bids can only be added to active auctions.");
        }

        _bids.Add(bid);
    }

    /// <summary>
    /// Performs the anti snipping operation.
    /// </summary>
    /// <param name="now">Date and time for the operation.</param>
    private void AntiSnipping(DateTime now)
    {
        if (EndDate == null)
        {
            throw new InvalidOperationException("Auction end date is not set.");
        }

        var timeRemaining = EndDate.Value - now;

        if (timeRemaining <= AntiSnipingWindow)
        {
            EndDate = EndDate.Value.Add(AntiSnipingExtension);
        }
    }
}

