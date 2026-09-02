using Relora.Items.Application.Models;
using Relora.Items.Domain.Enums;
using Relora.Shared.Domain.Constants;
using Relora.Shared.Domain.Enums;
using Relora.Shared.Domain.Page;

using MediatR;

namespace Relora.Items.Application.Queries;

/// <summary>
/// Represents a request for a filtered and sorted list of lots.
/// </summary>
public sealed record GetLotsListQuery
    : IRequest<PagedResult<LotPreviewDto>>
{
    public string? Search { get; init; }

    public LotCategory[] Categories { get; init; } = [];

    public LotGender[] Genders { get; init; } = [];

    public LotDepartment[] Departments { get; init; } = [];

    public LotSize[] Sizes { get; init; } = [];

    public LotCondition[] Conditions { get; init; } = [];

    public string[] Brands { get; init; } = [];

    public string[] Countries { get; init; } = [];

    public Guid[] CategoryIds { get; init; } = [];

    public string[] RootCategorySlugs { get; init; } = [];

    public string[] CategorySlugs { get; init; } = [];

    public Guid[] MaterialIds { get; init; } = [];

    public Guid[] PrimaryColorIds { get; init; } = [];

    public int? ProductionYearFrom { get; init; }

    public int? ProductionYearTo { get; init; }

    public bool VintageOnly { get; init; }

    public bool HasMeasurements { get; init; }

    public bool HasProofOfOrigin { get; init; }

    public decimal? MinPrice { get; init; }

    public decimal? MaxPrice { get; init; }

    public bool EndingSoon { get; init; }

    public bool NewlyListed { get; init; }

    public bool IncludeSold { get; init; }

    public LotSort Sort { get; init; } = LotSort.NewlyListed;

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 24;
}
