using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relora.Auctions.Application.Models;
/// <summary>
/// Represents the bids by auction dto class.
/// </summary>
public sealed class BidsByAuctionDto
{
    /// <summary>
    /// Gets or sets the bid id used by this type.
    /// </summary>
    public Guid BidId { get; init; }
    /// <summary>
    /// Gets or sets the user id used by this type.
    /// </summary>
    public Guid UserId { get; init; }
    /// <summary>
    /// Gets or sets the amount used by this type.
    /// </summary>
    public decimal Amount { get; init; }
    /// <summary>
    /// Gets or sets the currency used by this type.
    /// </summary>
    public string Currency { get; init; } = default!;
    /// <summary>
    /// Gets or sets the status used by this type.
    /// </summary>
    public string Status { get; init; } = default!;
    /// <summary>
    /// Gets or sets the placed at used by this type.
    /// </summary>
    public DateTime PlacedAt { get; init; }
}
