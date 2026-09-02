using Relora.Items.Application.Interfaces;
using Relora.Items.Application.Models;
using Relora.Items.Domain;
using Relora.Items.Domain.Enums;

using static Relora.Items.Domain.Lot;

namespace Relora.Items.Application.Services;

public sealed record ResolvedLotCatalogInput(
    Guid CategoryId,
    LotDepartment Department,
    LotCategory LegacyCategory,
    LotGender LegacyGender,
    Guid PrimaryColorId,
    IReadOnlyList<LotMaterial> Materials,
    IReadOnlyList<LotMeasurement> Measurements);

public static class LotCatalogInputResolver
{
    public static async Task<ResolvedLotCatalogInput> ResolveAsync(
        ILotCatalogRepository catalogRepository,
        Guid? categoryId,
        LotDepartment? department,
        LotCategory legacyCategory,
        LotGender legacyGender,
        Guid? primaryColorId,
        string? legacyColor,
        IReadOnlyList<LotMaterialInput>? materials,
        IReadOnlyList<LotMeasurementInput>? measurements,
        CancellationToken cancellationToken)
    {
        var resolvedDepartment = department ?? MapDepartment(legacyGender);
        var category = await ResolveCategoryAsync(catalogRepository, categoryId, legacyCategory, cancellationToken);

        if (!category.IsActive)
        {
            throw new InvalidOperationException("Selected category is not active.");
        }

        if (!category.AllowsDepartment(resolvedDepartment))
        {
            throw new InvalidOperationException("Selected category is not available for the selected department.");
        }

        var resolvedPrimaryColor = await ResolvePrimaryColorAsync(
            catalogRepository,
            primaryColorId,
            legacyColor,
            cancellationToken);

        if (!resolvedPrimaryColor.IsActive)
        {
            throw new InvalidOperationException("Selected primary color is not active.");
        }

        var resolvedMaterials = await ResolveMaterialsAsync(
            catalogRepository,
            materials,
            cancellationToken);

        var resolvedMeasurements = await ResolveMeasurementsAsync(
            catalogRepository,
            category.MeasurementProfile,
            measurements,
            cancellationToken);

        return new ResolvedLotCatalogInput(
            category.Id,
            resolvedDepartment,
            legacyCategory == LotCategory.Vintage ? LotCategory.Other : legacyCategory,
            MapLegacyGender(resolvedDepartment),
            resolvedPrimaryColor.Id,
            resolvedMaterials,
            resolvedMeasurements);
    }

    public static LotDepartment MapDepartment(LotGender legacyGender)
    {
        return legacyGender switch
        {
            LotGender.Women => LotDepartment.Women,
            LotGender.Men => LotDepartment.Men,
            LotGender.Unisex => LotDepartment.Unisex,
            _ => LotDepartment.Unisex
        };
    }

    private static LotGender MapLegacyGender(LotDepartment department)
    {
        return department switch
        {
            LotDepartment.Women => LotGender.Women,
            LotDepartment.Men => LotGender.Men,
            LotDepartment.Unisex => LotGender.Unisex,
            _ => LotGender.Unisex
        };
    }

    private static async Task<Category> ResolveCategoryAsync(
        ILotCatalogRepository catalogRepository,
        Guid? categoryId,
        LotCategory legacyCategory,
        CancellationToken cancellationToken)
    {
        if (categoryId.HasValue && categoryId.Value != Guid.Empty)
        {
            return await catalogRepository.GetCategoryByIdAsync(categoryId.Value, cancellationToken)
                ?? throw new InvalidOperationException("Selected category was not found.");
        }

        var fallbackSlug = legacyCategory switch
        {
            LotCategory.Tops => "other-clothing",
            LotCategory.Bottoms => "other-clothing",
            LotCategory.Outerwear => "other-clothing",
            LotCategory.Shoes => "other-shoes",
            LotCategory.Accessories => "other-accessories",
            LotCategory.Bags => "other-bags",
            LotCategory.Jewellery => "other-jewellery",
            LotCategory.Vintage => "other-clothing",
            _ => "other-clothing"
        };

        return await catalogRepository.GetCategoryBySlugAsync(fallbackSlug, cancellationToken)
            ?? throw new InvalidOperationException("Default category lookup data is missing.");
    }

    private static async Task<ItemColor> ResolvePrimaryColorAsync(
        ILotCatalogRepository catalogRepository,
        Guid? primaryColorId,
        string? legacyColor,
        CancellationToken cancellationToken)
    {
        if (primaryColorId.HasValue && primaryColorId.Value != Guid.Empty)
        {
            return await catalogRepository.GetColorByIdAsync(primaryColorId.Value, cancellationToken)
                ?? throw new InvalidOperationException("Selected primary color was not found.");
        }

        var fallbackCode = NormalizeColorCode(legacyColor);

        return await catalogRepository.GetColorByCodeAsync(fallbackCode, cancellationToken)
            ?? await catalogRepository.GetColorByCodeAsync("other", cancellationToken)
            ?? throw new InvalidOperationException("Default color lookup data is missing.");
    }

    private static async Task<IReadOnlyList<LotMaterial>> ResolveMaterialsAsync(
        ILotCatalogRepository catalogRepository,
        IReadOnlyList<LotMaterialInput>? materials,
        CancellationToken cancellationToken)
    {
        if (materials is null || materials.Count == 0)
        {
            return [];
        }

        var materialIds = materials
            .Select(material => material.MaterialId)
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToArray();

        if (materialIds.Length != materials.Count)
        {
            throw new InvalidOperationException("Material ids must be unique and non-empty.");
        }

        var knownMaterials = await catalogRepository.GetMaterialsByIdsAsync(materialIds, cancellationToken);

        if (knownMaterials.Count != materialIds.Length || knownMaterials.Any(material => !material.IsActive))
        {
            throw new InvalidOperationException("One or more selected materials are not available.");
        }

        return materials
            .Select(material => new LotMaterial(
                material.MaterialId,
                material.Percentage,
                material.OtherName))
            .ToList();
    }

    private static async Task<IReadOnlyList<LotMeasurement>> ResolveMeasurementsAsync(
        ILotCatalogRepository catalogRepository,
        string measurementProfile,
        IReadOnlyList<LotMeasurementInput>? measurements,
        CancellationToken cancellationToken)
    {
        if (measurements is null || measurements.Count == 0)
        {
            return [];
        }

        var definitions = await catalogRepository.GetMeasurementDefinitionsByProfileAsync(
            measurementProfile,
            cancellationToken);

        var allowedKeys = definitions
            .Select(definition => definition.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (allowedKeys.Count == 0)
        {
            throw new InvalidOperationException("Selected category does not accept measurements.");
        }

        return measurements
            .Select(measurement =>
            {
                var key = measurement.Key.Trim().ToLowerInvariant();

                if (!allowedKeys.Contains(key))
                {
                    throw new InvalidOperationException($"Measurement '{measurement.Key}' is not allowed for the selected category.");
                }

                return new LotMeasurement(
                    key,
                    measurement.Value,
                    string.IsNullOrWhiteSpace(measurement.Unit) ? "cm" : measurement.Unit);
            })
            .ToList();
    }

    private static string NormalizeColorCode(string? color)
    {
        if (string.IsNullOrWhiteSpace(color))
        {
            return "other";
        }

        var normalized = color.Trim().ToLowerInvariant().Replace(" ", "-");

        return normalized switch
        {
            "gray" => "grey",
            "navy-blue" => "navy",
            "multi" => "multicolor",
            "multi-color" => "multicolor",
            _ => normalized
        };
    }
}
