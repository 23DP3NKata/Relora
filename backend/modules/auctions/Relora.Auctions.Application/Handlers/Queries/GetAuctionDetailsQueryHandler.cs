using Relora.Auctions.Application.Models;
using Relora.Auctions.Application.Queries;
using Relora.Persistance;
using Relora.Shared.Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Relora.Auctions.Application.Handlers.Queries;

/// <summary>
/// Represents the get auction details query handler class.
/// </summary>
public sealed class GetAuctionDetailsQueryHandler
    : IRequestHandler<GetAuctionDetailsQuery, AuctionDetailsDto>
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
    /// Initializes a new instance of the <see cref="GetAuctionDetailsQueryHandler"/> class.
    /// </summary>
    /// <param name="context">Context.</param>
    public GetAuctionDetailsQueryHandler(ReloraDbContext context)
    {
        _context = context;
    }

    public async Task<AuctionDetailsDto> Handle(GetAuctionDetailsQuery request, CancellationToken cancellationToken)
    {
        var dto = await _context.Auctions
            .AsNoTracking()
            .Where(a => a.Id == request.AuctionId)
            .Where(a => a.LotId.HasValue)
            .Join(
                _context.Lots.AsNoTracking(),
                auction => auction.LotId!.Value,
                lot => lot.Id,
                (auction, lot) => new { Auction = auction, Lot = lot })
            .Where(item =>
                request.ViewerIsAdmin ||
                (request.ViewerUserId.HasValue && item.Lot.SellerId == request.ViewerUserId.Value) ||
                (
                    PublicLotStatuses.Contains(item.Lot.Status) &&
                    PublicAuctionStatuses.Contains(item.Auction.Status)
                ))
            .Select(item => new AuctionDetailsDto
            {
                AuctionId = item.Auction.Id,
                Status = item.Auction.Status.ToString(),

                CurrentPrice = item.Auction.CurrentPrice != null
                    ? item.Auction.CurrentPrice.Amount
                    : 0m,

                Currency = item.Auction.CurrentPrice != null
                    ? item.Auction.CurrentPrice.Currency
                    : null,

                StartsAt = item.Auction.StartDate,
                EndsAt = item.Auction.EndDate,

                LotId = item.Auction.LotId,
                LotTitle = item.Lot.Title
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (dto is null)
        {
            throw new KeyNotFoundException("Auction not found.");
        }

        return dto;
    }

}
