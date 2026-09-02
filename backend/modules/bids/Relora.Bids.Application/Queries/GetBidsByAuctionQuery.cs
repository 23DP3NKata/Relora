using Relora.Bids.Application.Models;

using MediatR;

namespace Relora.Bids.Application.Queries;

/// <summary>
/// Represents the get bids by auction query record.
/// </summary>
public sealed record GetBidsByAuctionQuery(
    Guid AuctionId,
    Guid? ViewerUserId = null,
    bool ViewerIsAdmin = false) : IRequest<IReadOnlyList<BidsByAuctionDto>>;
