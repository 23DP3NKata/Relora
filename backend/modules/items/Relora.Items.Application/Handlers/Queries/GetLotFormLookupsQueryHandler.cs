using Relora.Items.Application.Interfaces;
using Relora.Items.Application.Models;
using Relora.Items.Application.Queries;
using Relora.Items.Domain;
using Relora.Items.Domain.Enums;

using MediatR;

namespace Relora.Items.Application.Handlers.Queries;

public sealed class GetLotFormLookupsQueryHandler(ILotCatalogRepository catalogRepository)
    : IRequestHandler<GetLotFormLookupsQuery, LotFormLookupsDto>
{
    private readonly ILotCatalogRepository _catalogRepository = catalogRepository;

    public async Task<LotFormLookupsDto> Handle(GetLotFormLookupsQuery request, CancellationToken cancellationToken)
    {
        var categories = await _catalogRepository.GetCategoriesAsync(cancellationToken);
        var materials = await _catalogRepository.GetMaterialsAsync(cancellationToken);
        var colors = await _catalogRepository.GetColorsAsync(cancellationToken);
        var proofTypes = await _catalogRepository.GetProofDocumentTypesAsync(cancellationToken);
        var measurementDefinitions = await _catalogRepository.GetMeasurementDefinitionsAsync(cancellationToken);

        return new LotFormLookupsDto(
            Enum.GetValues<LotDepartment>(),
            BuildCategoryTree(categories),
            materials.Select(material => new LookupOptionDto(
                material.Id,
                material.Code,
                material.NameKey,
                material.SortOrder,
                material.IsActive)).ToList(),
            colors.Select(color => new LookupOptionDto(
                color.Id,
                color.Code,
                color.NameKey,
                color.SortOrder,
                color.IsActive)).ToList(),
            proofTypes.Select(type => new LookupOptionDto(
                type.Id,
                type.Code,
                type.NameKey,
                type.SortOrder,
                type.IsActive)).ToList(),
            measurementDefinitions.Select(definition => new MeasurementDefinitionDto(
                definition.Id,
                definition.Profile,
                definition.Key,
                definition.LabelKey,
                definition.IsRequired,
                definition.SortOrder,
                definition.Unit)).ToList(),
            BuildSizeOptions());
    }

    private static IReadOnlyList<CategoryDto> BuildCategoryTree(IReadOnlyList<Category> categories)
    {
        return categories
            .Where(category => category.ParentId is null)
            .OrderBy(category => category.SortOrder)
            .Select(root => ToDto(root, categories))
            .ToList();
    }

    private static CategoryDto ToDto(Category category, IReadOnlyList<Category> categories)
    {
        var children = categories
            .Where(candidate => candidate.ParentId == category.Id)
            .OrderBy(candidate => candidate.SortOrder)
            .ThenBy(candidate => candidate.NameKey)
            .Select(child => ToDto(child, categories))
            .ToList();

        return new CategoryDto(
            category.Id,
            category.ParentId,
            category.Slug,
            category.NameKey,
            category.SortOrder,
            category.IsActive,
            category.MeasurementProfile,
            category.AllowedDepartments.Select(item => item.Department).ToList(),
            children);
    }

    private static IReadOnlyList<SizeOptionDto> BuildSizeOptions()
    {
        return
        [
            new("xxs", "sizes.xxs", "clothing"),
            new("xs", "sizes.xs", "clothing"),
            new("s", "sizes.s", "clothing"),
            new("m", "sizes.m", "clothing"),
            new("l", "sizes.l", "clothing"),
            new("xl", "sizes.xl", "clothing"),
            new("xxl", "sizes.xxl", "clothing"),
            new("xxxl", "sizes.xxxl", "clothing"),
            new("eu-35", "sizes.eu35", "shoes"),
            new("eu-36", "sizes.eu36", "shoes"),
            new("eu-37", "sizes.eu37", "shoes"),
            new("eu-38", "sizes.eu38", "shoes"),
            new("eu-39", "sizes.eu39", "shoes"),
            new("eu-40", "sizes.eu40", "shoes"),
            new("eu-41", "sizes.eu41", "shoes"),
            new("eu-42", "sizes.eu42", "shoes"),
            new("eu-43", "sizes.eu43", "shoes"),
            new("eu-44", "sizes.eu44", "shoes"),
            new("eu-45", "sizes.eu45", "shoes"),
            new("one-size", "sizes.oneSize", "oneSize"),
            new("unknown", "sizes.unknown", "fallback"),
            new("not-applicable", "sizes.notApplicable", "fallback")
        ];
    }
}
