using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Relora.Auctions.Application.Models;
using Relora.Auctions.Application.Queries;
using Relora.Persistance;
using Relora.Shared.Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Relora.Auctions.Application.Handlers.Queries;

/// <summary>
/// Represents the get auctions list query handler class.
/// </summary>
public sealed class GetAuctionsListQueryHandler : IRequestHandler<GetAuctionsListQuery, IReadOnlyList<AuctionListItemDto>>
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
    /// Initializes a new instance of the <see cref="GetAuctionsListQueryHandler"/> class.
    /// </summary>
    /// <param name="context">Context.</param>
    public GetAuctionsListQueryHandler(ReloraDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<AuctionListItemDto>> Handle(
        GetAuctionsListQuery request,
        CancellationToken cancellationToken
    )
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var query = _context.Auctions
            .AsNoTracking()
            .Where(auction => auction.LotId.HasValue)
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
                ));

        if (Enum.TryParse<AuctionStatus>(request.Status, ignoreCase: true, out var status))
        {
            query = query.Where(item => item.Auction.Status == status);
        }

        return await query
            .OrderBy(item => item.Auction.EndDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(item => new AuctionListItemDto
            {
                AuctionId = item.Auction.Id,
                Status = item.Auction.Status.ToString(),
                CurrentPrice = item.Auction.CurrentPrice.Amount,
                Currency = item.Auction.CurrentPrice.Currency,
                EndDate = item.Auction.EndDate,
                LotId = item.Auction.LotId,
            })
            .ToListAsync(cancellationToken);
    }
}
