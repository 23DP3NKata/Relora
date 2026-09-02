using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relora.Auctions.Application.Models;
/// <summary>
/// Represents the auction details dto class.
/// </summary>
public sealed class AuctionDetailsDto
{
    /// <summary>
    /// Gets or sets the auction id used by this type.
    /// </summary>
    public Guid AuctionId { get; init; }
    /// <summary>
    /// Gets or sets the status used by this type.
    /// </summary>
    public string Status { get; init; } = default!;

    /// <summary>
    /// Gets or sets the current price used by this type.
    /// </summary>
    public decimal? CurrentPrice { get; init; }
    /// <summary>
    /// Gets or sets the currency used by this type.
    /// </summary>
    public string? Currency { get; init; } = default!;

    /// <summary>
    /// Gets or sets the starts at used by this type.
    /// </summary>
    public DateTime? StartsAt { get; init; }
    /// <summary>
    /// Gets or sets the ends at used by this type.
    /// </summary>
    public DateTime? EndsAt { get; init; }

    /// <summary>
    /// Gets or sets the lot id used by this type.
    /// </summary>
    public Guid? LotId { get; init; }
    /// <summary>
    /// Gets or sets the lot title used by this type.
    /// </summary>
    public string LotTitle { get; init; } = default!;
}
