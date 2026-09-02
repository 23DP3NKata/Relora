using Relora.Items.Application.Interfaces;
using Relora.Items.Domain;
using Relora.Persistance;

using Microsoft.EntityFrameworkCore;

namespace Relora.Items.Infrastructure.Repository;

public sealed class LotCatalogRepository(ReloraDbContext context) : ILotCatalogRepository
{
    private readonly ReloraDbContext _context = context;

    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .Include(category => category.AllowedDepartments)
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.NameKey)
            .ToListAsync(cancellationToken);
    }

    public Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Categories
            .AsNoTracking()
            .Include(category => category.AllowedDepartments)
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);
    }

    public Task<Category?> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        var normalizedSlug = slug.Trim().ToLowerInvariant();

        return _context.Categories
            .AsNoTracking()
            .Include(category => category.AllowedDepartments)
            .FirstOrDefaultAsync(category => category.Slug == normalizedSlug, cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesBySlugsAsync(
        IReadOnlyCollection<string> slugs,
        CancellationToken cancellationToken)
    {
        var normalizedSlugs = slugs
            .Where(slug => !string.IsNullOrWhiteSpace(slug))
            .Select(slug => slug.Trim().ToLowerInvariant())
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (normalizedSlugs.Length == 0)
        {
            return [];
        }

        return await _context.Categories
            .AsNoTracking()
            .Include(category => category.AllowedDepartments)
            .Where(category => normalizedSlugs.Contains(category.Slug))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Material>> GetMaterialsAsync(CancellationToken cancellationToken)
    {
        return await _context.Materials
            .AsNoTracking()
            .OrderBy(material => material.SortOrder)
            .ThenBy(material => material.NameKey)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Material>> GetMaterialsByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken)
    {
        var normalizedIds = ids.Where(id => id != Guid.Empty).Distinct().ToArray();

        if (normalizedIds.Length == 0)
        {
            return [];
        }

        return await _context.Materials
            .AsNoTracking()
            .Where(material => normalizedIds.Contains(material.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<Material?> GetMaterialByCodeAsync(string code, CancellationToken cancellationToken)
    {
        var normalizedCode = code.Trim().ToLowerInvariant();

        return _context.Materials
            .AsNoTracking()
            .FirstOrDefaultAsync(material => material.Code == normalizedCode, cancellationToken);
    }

    public async Task<IReadOnlyList<ItemColor>> GetColorsAsync(CancellationToken cancellationToken)
    {
        return await _context.Colors
            .AsNoTracking()
            .OrderBy(color => color.SortOrder)
            .ThenBy(color => color.NameKey)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ItemColor>> GetColorsByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken)
    {
        var normalizedIds = ids.Where(id => id != Guid.Empty).Distinct().ToArray();

        if (normalizedIds.Length == 0)
        {
            return [];
        }

        return await _context.Colors
            .AsNoTracking()
            .Where(color => normalizedIds.Contains(color.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<ItemColor?> GetColorByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Colors
            .AsNoTracking()
            .FirstOrDefaultAsync(color => color.Id == id, cancellationToken);
    }

    public Task<ItemColor?> GetColorByCodeAsync(string code, CancellationToken cancellationToken)
    {
        var normalizedCode = code.Trim().ToLowerInvariant();

        return _context.Colors
            .AsNoTracking()
            .FirstOrDefaultAsync(color => color.Code == normalizedCode, cancellationToken);
    }

    public async Task<IReadOnlyList<MeasurementDefinition>> GetMeasurementDefinitionsAsync(CancellationToken cancellationToken)
    {
        return await _context.MeasurementDefinitions
            .AsNoTracking()
            .OrderBy(definition => definition.Profile)
            .ThenBy(definition => definition.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MeasurementDefinition>> GetMeasurementDefinitionsByProfileAsync(
        string profile,
        CancellationToken cancellationToken)
    {
        var normalizedProfile = string.IsNullOrWhiteSpace(profile)
            ? "none"
            : profile.Trim().ToLowerInvariant();

        return await _context.MeasurementDefinitions
            .AsNoTracking()
            .Where(definition => definition.Profile == normalizedProfile && definition.IsActive)
            .OrderBy(definition => definition.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProofDocumentType>> GetProofDocumentTypesAsync(CancellationToken cancellationToken)
    {
        return await _context.ProofDocumentTypes
            .AsNoTracking()
            .OrderBy(type => type.SortOrder)
            .ThenBy(type => type.NameKey)
            .ToListAsync(cancellationToken);
    }

    public Task<ProofDocumentType?> GetProofDocumentTypeByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.ProofDocumentTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(type => type.Id == id, cancellationToken);
    }
}
