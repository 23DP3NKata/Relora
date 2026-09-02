using Relora.Auctions.Application.Models;
using Relora.Auctions.Application.Queries;
using Relora.Persistance;
using Relora.Shared.Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Relora.Auctions.Application.Handlers.Queries;

public sealed class GetAuctionByLotQueryHandler
    : IRequestHandler<GetAuctionByLotQuery, AuctionDetailsDto>
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

    public GetAuctionByLotQueryHandler(ReloraDbContext context)
    {
        _context = context;
    }

    public async Task<AuctionDetailsDto> Handle(
        GetAuctionByLotQuery request,
        CancellationToken cancellationToken)
    {
        var dto = await _context.Auctions
            .AsNoTracking()
            .Where(auction => auction.LotId == request.LotId)
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
            .OrderBy(item => item.Auction.Status == AuctionStatus.Active ? 0 : 1)
            .ThenByDescending(item => item.Auction.EndDate)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (dto is null)
        {
            throw new KeyNotFoundException("Auction not found.");
        }

        return dto;
    }
}
