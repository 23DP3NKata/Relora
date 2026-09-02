using MediatR;
using Relora.Auctions.Application.Models;

namespace Relora.Auctions.Application.Queries;

/// <summary>
/// Represents the get auction details query record.
/// </summary>
public record GetAuctionDetailsQuery(
    Guid AuctionId,
    Guid? ViewerUserId = null,
    bool ViewerIsAdmin = false) : IRequest<AuctionDetailsDto>;
