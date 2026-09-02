namespace Relora.Auctions.Application.Models;

/// <summary>
/// Represents the current price dto class.
/// </summary>
public sealed class CurrentPriceDto
{
    /// <summary>
    /// Gets or sets the auction id used by this type.
    /// </summary>
    public Guid AuctionId { get; init; }
    /// <summary>
    /// Gets or sets the price used by this type.
    /// </summary>
    public decimal Price { get; init; }
    /// <summary>
    /// Gets or sets the currency used by this type.
    /// </summary>
    public string Currency { get; init; } = default!;
}
