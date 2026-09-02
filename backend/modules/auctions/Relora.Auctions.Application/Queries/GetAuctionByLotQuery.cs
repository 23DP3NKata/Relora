using Relora.Auctions.Application.Models;

using MediatR;

namespace Relora.Auctions.Application.Queries;

public sealed record GetAuctionByLotQuery(
    Guid LotId,
    Guid? ViewerUserId = null,
    bool ViewerIsAdmin = false) : IRequest<AuctionDetailsDto>;
