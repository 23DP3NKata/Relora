using Relora.Auctions.Application.Models;

using MediatR;

namespace Relora.Auctions.Application.Queries;

/// <summary>
/// Represents the get auctions list query record.
/// </summary>
public sealed record GetAuctionsListQuery : IRequest<IReadOnlyList<AuctionListItemDto>>
{
    /// <summary>
    /// Gets or sets the page used by this type.
    /// </summary>
    public int Page { get; init; } = 1;
    /// <summary>
    /// Gets or sets the page size used by this type.
    /// </summary>
    public int PageSize { get; init; } = 20;
    /// <summary>
    /// Gets or sets the status used by this type.
    /// </summary>
    public string? Status { get; init; }
    /// <summary>
    /// Gets or sets the sort used by this type.
    /// </summary>
    public string? Sort { get; init; }

    public Guid? ViewerUserId { get; init; }

    public bool ViewerIsAdmin { get; init; }
}
