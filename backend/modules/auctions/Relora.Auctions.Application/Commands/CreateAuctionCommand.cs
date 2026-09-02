using MediatR;

namespace Relora.Auctions.Application.Commands;

/// <summary>
/// Represents the create auction command record.
/// </summary>
public sealed record CreateAuctionCommand(Guid LotId, Guid UserId, bool IsAdmin) : IRequest<Guid>;
