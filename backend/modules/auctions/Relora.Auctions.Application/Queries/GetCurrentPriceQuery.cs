using MediatR;
using Relora.Auctions.Application.Models;

namespace Relora.Auctions.Application.Queries;
/// <summary>
/// Represents the get current price query record.
/// </summary>
public sealed record GetCurrentPriceQuery(Guid AuctionId) : IRequest<CurrentPriceDto>;
