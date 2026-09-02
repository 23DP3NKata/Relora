using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relora.Auctions.Application.Models;

/// <summary>
/// Represents the auction list item dto class.
/// </summary>
public sealed class AuctionListItemDto
{
    /// <summary>
    /// Gets or sets the auction id used by this type.
    /// </summary>
    public Guid AuctionId { get; init; }
    /// <summary>
    /// Gets or sets the lot id used by this type.
    /// </summary>
    public Guid? LotId { get; init; } = default!;
    /// <summary>
    /// Gets or sets the status used by this type.
    /// </summary>
    public string Status { get; init; } = default!;
    /// <summary>
    /// Gets or sets the end date used by this type.
    /// </summary>
    public DateTime? EndDate { get; init; }
    /// <summary>
    /// Gets or sets the current price used by this type.
    /// </summary>
    public decimal? CurrentPrice { get; init; }
    /// <summary>
    /// Gets or sets the currency used by this type.
    /// </summary>
    public string? Currency { get; init; } = default!;
}
