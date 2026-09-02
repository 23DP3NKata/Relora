using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Relora.Bids.Application.Models;
using Relora.Bids.Application.Queries;
using Relora.Persistance;
using Relora.Shared.Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Relora.Bids.Application.Handlers.Queries;

/// <summary>
/// Represents the get bids by auction query handler class.
/// </summary>
public sealed class GetBidsByAuctionQueryHandler : IRequestHandler<GetBidsByAuctionQuery, IReadOnlyList<BidsByAuctionDto>>
{
    private static readonly LotStatus[] PublicLotStatuses =
    [
        LotStatus.Listed,
        LotStatus.Sold
    ];

    private static readonly AuctionStatus[] PublicAuctionStatuses =
    [
        AuctionStatus.Active,
        AuctionStatus.Finished,
        AuctionStatus.Unsold
    ];

    private readonly ReloraDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetBidsByAuctionQueryHandler"/> class.
    /// </summary>
    /// <param name="context">Context.</param>
    public GetBidsByAuctionQueryHandler(ReloraDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the operation.
    /// </summary>
    /// <param name="request">Input data for the operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<IReadOnlyList<BidsByAuctionDto>> Handle(GetBidsByAuctionQuery request, CancellationToken cancellationToken)
    {
        var canSeeAuction = await _context.Auctions
            .AsNoTracking()
            .Where(auction => auction.Id == request.AuctionId)
            .Where(auction => auction.LotId.HasValue)
            .Join(
                _context.Lots.AsNoTracking(),
                auction => auction.LotId!.Value,
                lot => lot.Id,
                (auction, lot) => new { Auction = auction, Lot = lot })
            .AnyAsync(item =>
                request.ViewerIsAdmin ||
                (request.ViewerUserId.HasValue && item.Lot.SellerId == request.ViewerUserId.Value) ||
                (
                    PublicLotStatuses.Contains(item.Lot.Status) &&
                    PublicAuctionStatuses.Contains(item.Auction.Status)
                ),
                cancellationToken);

        if (!canSeeAuction)
        {
            throw new KeyNotFoundException("Auction not found.");
        }

        return await _context.Bids
           .AsNoTracking()
           .Where(b => b.AuctionId == request.AuctionId)
           .OrderByDescending(b => b.PlacedAt)
           .Select(b => new BidsByAuctionDto
           {
               BidId = b.Id,
               UserId = b.BidderId,
               Amount = b.Amount.Amount,
               Currency = b.Amount.Currency,
               Status = b.Status.ToString(),
               PlacedAt = b.PlacedAt
           })
           .ToListAsync(cancellationToken);
    }
}
