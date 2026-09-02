using Relora.Shared.Domain.Enums;

using MediatR;

namespace Relora.Auctions.Application.Commands;

/// <summary>
/// Represents the start auction command record.
/// </summary>
public sealed record StartAuctionCommand(
    Guid AuctionId,
    AuctionDurationOption Duration,
    Guid UserId,
    bool IsAdmin) : IRequest;
