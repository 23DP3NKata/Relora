using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Relora.Bids.Application.Commands;

/// <summary>
/// Represents the place bid command record.
/// </summary>
public sealed record PlaceBidCommand(
    Guid AuctionId,

    [param: Required]
    Guid BidderId,

    decimal Amount,

    [param: Required]
    [param: RegularExpression("^(USD|EUR|GBP|JPY|AUD|CAD|CHF|CNY|SEK|NZD)$")]
    string Currency
) : IRequest;
