using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Relora.Identity.Application.Interfaces;
using Relora.Items.Application.Interfaces;
using Relora.Items.Application.Models;
using Relora.Items.Application.Queries;
using Relora.Items.Application.Services;
using Relora.Items.Domain;
using Relora.Shared.Domain.Enums;
using Relora.Shared.Domain.Page;
using Relora.Shared.Infrastructure.Media;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Relora.Items.Application.Handlers.Queries;

/// <summary>
/// Represents the get lots list query handler class.
/// </summary>
public class GetLotsListQueryHandler : IRequestHandler<GetLotsListQuery, PagedResult<LotPreviewDto>>
{
    private readonly ILotRepository _lotRepository;
    private readonly ILotCatalogRepository _catalogRepository;
    private readonly IUserRepository _userRepository;
    private readonly string _publicBaseUrl;


    /// <summary>
    /// Initializes a new instance of the <see cref="GetLotsListQueryHandler"/> class.
    /// </summary>
    /// <param name="lotRepository">Lot repository.</param>
    public GetLotsListQueryHandler(
        ILotRepository lotRepository,
        ILotCatalogRepository catalogRepository,
        IUserRepository userRepository,
        IOptions<MediaOptions> mediaOptions)
    {
        _lotRepository = lotRepository;
        _catalogRepository = catalogRepository;
        _userRepository = userRepository;
        _publicBaseUrl = mediaOptions.Value.PublicBaseUrl;
    }

    /// <summary>
    /// Handles the operation.
    /// </summary>
    /// <param name="request">Input data for the operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<PagedResult<LotPreviewDto>> Handle(
    GetLotsListQuery request,
    CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;

        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var query = _lotRepository
            .GetQueryable()
            .AsNoTracking();

        query = query.Where(lot =>
                lot.Status == LotStatus.Listed ||
                lot.Status == LotStatus.Sold);

        query = query.ApplySearch(request.Search);

        query = await ApplyCategorySlugFiltersAsync(query, request, cancellationToken);

        query = query.ApplyCatalogFilters(request, utcNow);

        var totalCount = await query.CountAsync(cancellationToken);

        var orderedQuery = query.ApplyCatalogSorting(request.Sort);

        var lots = await orderedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var catalogContext = await CreateCatalogContextAsync(cancellationToken);
        var items = lots
            .Select(lot => LotReadModelMapper.ToPreview(lot, catalogContext, _publicBaseUrl))
            .ToList();

        return new PagedResult<LotPreviewDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private async Task<LotCatalogReadContext> CreateCatalogContextAsync(CancellationToken cancellationToken)
    {
        var categories = await _catalogRepository.GetCategoriesAsync(cancellationToken);
        var colors = await _catalogRepository.GetColorsAsync(cancellationToken);
        var materials = await _catalogRepository.GetMaterialsAsync(cancellationToken);
        var proofTypes = await _catalogRepository.GetProofDocumentTypesAsync(cancellationToken);
        var measurements = await _catalogRepository.GetMeasurementDefinitionsAsync(cancellationToken);

        return LotReadModelMapper.CreateContext(categories, colors, materials, proofTypes, measurements);
    }

    private async Task<IQueryable<Lot>> ApplyCategorySlugFiltersAsync(
        IQueryable<Lot> query,
        GetLotsListQuery request,
        CancellationToken cancellationToken)
    {
        if (request.RootCategorySlugs.Length == 0 && request.CategorySlugs.Length == 0)
        {
            return query;
        }

        var categories = await _catalogRepository.GetCategoriesAsync(cancellationToken);
        var selectedIds = new HashSet<Guid>();

        if (request.CategorySlugs.Length > 0)
        {
            var categorySlugs = request.CategorySlugs
                .Where(slug => !string.IsNullOrWhiteSpace(slug))
                .Select(slug => slug.Trim().ToLowerInvariant())
                .ToHashSet(StringComparer.Ordinal);

            foreach (var category in categories.Where(category => categorySlugs.Contains(category.Slug)))
            {
                selectedIds.Add(category.Id);
            }
        }

        if (request.RootCategorySlugs.Length > 0)
        {
            var rootSlugs = request.RootCategorySlugs
                .Where(slug => !string.IsNullOrWhiteSpace(slug))
                .Select(slug => slug.Trim().ToLowerInvariant())
                .ToHashSet(StringComparer.Ordinal);

            foreach (var root in categories.Where(category => category.ParentId is null && rootSlugs.Contains(category.Slug)))
            {
                selectedIds.Add(root.Id);
                foreach (var descendantId in GetDescendantIds(root.Id, categories))
                {
                    selectedIds.Add(descendantId);
                }
            }
        }

        if (selectedIds.Count == 0)
        {
            return query.Where(lot => false);
        }

        var categoryIds = selectedIds.ToArray();
        return query.Where(lot => categoryIds.Contains(lot.CategoryId));
    }

    private static IEnumerable<Guid> GetDescendantIds(Guid parentId, IReadOnlyList<Category> categories)
    {
        foreach (var child in categories.Where(category => category.ParentId == parentId))
        {
            yield return child.Id;

            foreach (var descendantId in GetDescendantIds(child.Id, categories))
            {
                yield return descendantId;
            }
        }
    }
}
