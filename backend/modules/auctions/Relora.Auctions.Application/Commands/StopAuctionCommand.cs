using MediatR;

namespace Relora.Auctions.Application.Commands;

/// <summary>
/// Represents the stop auction command record.
/// </summary>
public record StopAuctionCommand(
    Guid AuctionId,
    Guid? UserId = null,
    bool IsAdmin = false,
    bool IsSystem = false) : IRequest;
