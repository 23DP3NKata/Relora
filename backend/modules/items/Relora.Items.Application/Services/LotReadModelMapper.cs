using Relora.Items.Application.Models;
using Relora.Items.Domain;
using Relora.Items.Domain.Enums;
using Relora.Shared.Domain.Enums;

namespace Relora.Items.Application.Services;

public sealed record LotCatalogReadContext(
    IReadOnlyDictionary<Guid, Category> Categories,
    IReadOnlyDictionary<Guid, ItemColor> Colors,
    IReadOnlyDictionary<Guid, Material> Materials,
    IReadOnlyDictionary<Guid, ProofDocumentType> ProofDocumentTypes,
    IReadOnlyDictionary<string, MeasurementDefinition> MeasurementDefinitions);

public static class LotReadModelMapper
{
    public static LotPreviewDto ToPreview(Lot lot, LotCatalogReadContext catalog, string publicBaseUrl)
    {
        var category = ResolveCategory(lot, catalog);
        var root = ResolveRootCategory(category, catalog);

        return new LotPreviewDto
        {
            Id = lot.Id,
            Title = lot.Title,
            Price = lot.Price.Amount,
            Currency = lot.Price.Currency,
            Category = lot.Category,
            CategoryName = lot.Category.ToString(),
            Gender = lot.Gender,
            GenderName = lot.Gender.ToString(),
            Size = lot.Size,
            SizeName = lot.Size.ToString(),
            Brand = lot.Brand,
            Condition = lot.Condition,
            ConditionName = lot.Condition.ToString(),
            Color = lot.Color,
            CategoryId = lot.CategoryId,
            RootCategoryId = root?.Id,
            CategorySlug = category?.Slug,
            CategoryNameKey = category?.NameKey,
            RootCategorySlug = root?.Slug,
            RootCategoryNameKey = root?.NameKey,
            Department = lot.Department,
            DepartmentName = lot.Department.ToString(),
            PrimaryColorId = lot.PrimaryColorId,
            ProductionYear = lot.ProductionYear,
            IsVintage = lot.IsVintage,
            HasMeasurements = lot.Measurements.Count > 0,
            ProofStatus = lot.ProofDocuments.Count > 0 ? "submitted" : "notProvided",
            Status = lot.Status,
            StatusName = lot.Status.ToString(),
            ShippingPrice = lot.ShippingPrice,
            ShippingCurrency = lot.ShippingCurrency,
            ShippingOriginCountry = lot.ShippingOriginCountry,
            ShipsToCountries = lot.ShipsToCountries,
            ShippingHandlingDays = lot.ShippingHandlingDays,
            Media = ToMediaDtos(lot.Media, publicBaseUrl)
                .Take(3)
                .ToList()
        };
    }

    public static LotDto ToDetails(
        Lot lot,
        LotCatalogReadContext catalog,
        string publicBaseUrl,
        bool includePrivateProofDocuments,
        LotSellerDto seller)
    {
        var category = ResolveCategory(lot, catalog);
        var root = ResolveRootCategory(category, catalog);

        return new LotDto
        {
            Id = lot.Id,
            Title = lot.Title,
            Description = lot.Description,
            Price = lot.Price.Amount,
            Currency = lot.Price.Currency,
            Category = lot.Category,
            CategoryName = lot.Category.ToString(),
            Gender = lot.Gender,
            GenderName = lot.Gender.ToString(),
            Size = lot.Size,
            SizeName = lot.Size.ToString(),
            Brand = lot.Brand,
            Condition = lot.Condition,
            ConditionName = lot.Condition.ToString(),
            Color = lot.Color,
            CategoryId = lot.CategoryId,
            RootCategoryId = root?.Id,
            CategorySlug = category?.Slug,
            CategoryNameKey = category?.NameKey,
            RootCategorySlug = root?.Slug,
            RootCategoryNameKey = root?.NameKey,
            Department = lot.Department,
            DepartmentName = lot.Department.ToString(),
            PrimaryColorId = lot.PrimaryColorId,
            PrimaryColor = ToColorDto(lot.PrimaryColorId, catalog),
            SecondaryColors = lot.SecondaryColors
                .Select(color => ToColorDto(color.ColorId, catalog))
                .Where(color => color is not null)
                .Select(color => color!)
                .ToList(),
            ModelName = lot.ModelName,
            AcquisitionYear = lot.AcquisitionYear,
            ProductionYear = lot.ProductionYear,
            IsVintage = lot.IsVintage,
            VintageNotes = includePrivateProofDocuments ? lot.VintageNotes : null,
            Materials = lot.Materials.Select(material => ToMaterialDto(material, catalog)).ToList(),
            Measurements = lot.Measurements
                .Select(measurement => ToMeasurementDto(measurement, category, catalog))
                .ToList(),
            ProofStatus = lot.ProofDocuments.Count > 0 ? "submitted" : "notProvided",
            ProofDocuments = includePrivateProofDocuments
                ? lot.ProofDocuments.Select(document => ToProofDocumentDto(document, catalog)).ToList()
                : [],
            Status = lot.Status,
            StatusName = lot.Status.ToString(),
            CreatedAt = lot.CreatedAt,
            Age = lot.Age,
            Style = lot.Style,
            Country = lot.Country,
            City = lot.City,
            ShippingPrice = lot.ShippingPrice,
            ShippingCurrency = lot.ShippingCurrency,
            ShippingOriginCountry = lot.ShippingOriginCountry,
            ShipsToCountries = lot.ShipsToCountries,
            ShippingHandlingDays = lot.ShippingHandlingDays,
            Media = ToMediaDtos(lot.Media, publicBaseUrl).ToList(),
            Seller = seller
        };
    }

    public static MyLotListItemDto ToMyLot(Lot lot, LotCatalogReadContext catalog, string publicBaseUrl)
    {
        var preview = ToPreview(lot, catalog, publicBaseUrl);
        var category = ResolveCategory(lot, catalog);
        var root = ResolveRootCategory(category, catalog);

        return new MyLotListItemDto
        {
            Id = lot.Id,
            SellerId = lot.SellerId,
            Title = lot.Title,
            Description = lot.Description,
            Price = lot.Price.Amount,
            Currency = lot.Price.Currency,
            Category = lot.Category,
            CategoryName = lot.Category.ToString(),
            Gender = lot.Gender,
            GenderName = lot.Gender.ToString(),
            Size = lot.Size,
            SizeName = lot.Size.ToString(),
            Brand = lot.Brand,
            Condition = lot.Condition,
            ConditionName = lot.Condition.ToString(),
            Color = lot.Color,
            CategoryId = lot.CategoryId,
            RootCategoryId = root?.Id,
            CategorySlug = category?.Slug,
            CategoryNameKey = category?.NameKey,
            RootCategorySlug = root?.Slug,
            RootCategoryNameKey = root?.NameKey,
            Department = lot.Department,
            DepartmentName = lot.Department.ToString(),
            PrimaryColorId = lot.PrimaryColorId,
            ModelName = lot.ModelName,
            AcquisitionYear = lot.AcquisitionYear,
            ProductionYear = lot.ProductionYear,
            IsVintage = lot.IsVintage,
            VintageNotes = lot.VintageNotes,
            Materials = lot.Materials.Select(material => ToMaterialDto(material, catalog)).ToList(),
            SecondaryColors = lot.SecondaryColors
                .Select(color => ToColorDto(color.ColorId, catalog))
                .Where(color => color is not null)
                .Select(color => color!)
                .ToList(),
            Measurements = lot.Measurements
                .Select(measurement => ToMeasurementDto(measurement, category, catalog))
                .ToList(),
            ProofStatus = preview.ProofStatus,
            ProofDocuments = lot.ProofDocuments.Select(document => ToProofDocumentDto(document, catalog)).ToList(),
            Status = lot.Status,
            StatusName = lot.Status.ToString(),
            AuctionId = null,
            CreatedAt = lot.CreatedAt,
            ShippingPrice = lot.ShippingPrice,
            ShippingCurrency = lot.ShippingCurrency,
            ShippingOriginCountry = lot.ShippingOriginCountry,
            ShipsToCountries = lot.ShipsToCountries,
            ShippingHandlingDays = lot.ShippingHandlingDays,
            ModerationRejectionReason = lot.ModerationRejectionReason,
            ModerationReviewedByAdminId = lot.ModerationReviewedByAdminId,
            ModerationReviewedAtUtc = lot.ModerationReviewedAtUtc,
            ModerationMessage = lot.Status switch
            {
                LotStatus.Rejected => lot.ModerationRejectionReason ?? "Lot was rejected by moderation.",
                LotStatus.Published when lot.ModerationReviewedAtUtc is not null => "Lot approved and published.",
                LotStatus.Pending => "Lot is waiting for moderation.",
                _ => null
            },
            Media = preview.Media
        };
    }

    public static LotCatalogReadContext CreateContext(
        IReadOnlyList<Category> categories,
        IReadOnlyList<ItemColor> colors,
        IReadOnlyList<Material> materials,
        IReadOnlyList<ProofDocumentType> proofDocumentTypes,
        IReadOnlyList<MeasurementDefinition> measurementDefinitions)
    {
        return new LotCatalogReadContext(
            categories.ToDictionary(category => category.Id),
            colors.ToDictionary(color => color.Id),
            materials.ToDictionary(material => material.Id),
            proofDocumentTypes.ToDictionary(type => type.Id),
            measurementDefinitions.ToDictionary(
                definition => $"{definition.Profile}:{definition.Key}",
                StringComparer.OrdinalIgnoreCase));
    }

    private static Category? ResolveCategory(Lot lot, LotCatalogReadContext catalog)
    {
        return catalog.Categories.TryGetValue(lot.CategoryId, out var category)
            ? category
            : null;
    }

    private static Category? ResolveRootCategory(Category? category, LotCatalogReadContext catalog)
    {
        if (category is null)
        {
            return null;
        }

        if (category.ParentId is null)
        {
            return category;
        }

        return catalog.Categories.TryGetValue(category.ParentId.Value, out var parent)
            ? ResolveRootCategory(parent, catalog)
            : category;
    }

    private static LotMaterialDto ToMaterialDto(Lot.LotMaterial material, LotCatalogReadContext catalog)
    {
        if (!catalog.Materials.TryGetValue(material.MaterialId, out var lookup))
        {
            return new LotMaterialDto(material.MaterialId, "unknown", "materials.unknown", material.Percentage, material.OtherName);
        }

        return new LotMaterialDto(
            lookup.Id,
            lookup.Code,
            lookup.NameKey,
            material.Percentage,
            material.OtherName);
    }

    private static LotColorDto? ToColorDto(Guid colorId, LotCatalogReadContext catalog)
    {
        return catalog.Colors.TryGetValue(colorId, out var color)
            ? new LotColorDto(color.Id, color.Code, color.NameKey)
            : null;
    }

    private static IEnumerable<LotMediaDto> ToMediaDtos(IEnumerable<Lot.LotMedia> mediaItems, string publicBaseUrl)
    {
        return mediaItems
            .OrderByDescending(media => media.IsCover)
            .ThenBy(media => media.SortOrder)
            .ThenBy(media => media.Id)
            .Select(media => new LotMediaDto
            {
                Id = media.Id,
                Key = media.Key,
                Type = media.Type,
                Url = $"{publicBaseUrl}{media.Key}",
                SortOrder = media.SortOrder,
                IsCover = media.IsCover
            });
    }

    private static LotMeasurementDto ToMeasurementDto(
        Lot.LotMeasurement measurement,
        Category? category,
        LotCatalogReadContext catalog)
    {
        var definitionKey = category is null
            ? string.Empty
            : $"{category.MeasurementProfile}:{measurement.Key}";

        var labelKey = catalog.MeasurementDefinitions.TryGetValue(definitionKey, out var definition)
            ? definition.LabelKey
            : null;

        return new LotMeasurementDto(measurement.Key, measurement.Value, measurement.Unit, labelKey);
    }

    private static LotProofDocumentDto ToProofDocumentDto(
        Lot.LotProofDocument document,
        LotCatalogReadContext catalog)
    {
        if (!catalog.ProofDocumentTypes.TryGetValue(document.DocumentTypeId, out var type))
        {
            return new LotProofDocumentDto(
                document.Id,
                document.DocumentTypeId,
                "unknown",
                "proofDocuments.unknown",
                document.OriginalFileName,
                document.MimeType,
                document.SizeBytes,
                document.CreatedAtUtc);
        }

        return new LotProofDocumentDto(
            document.Id,
            type.Id,
            type.Code,
            type.NameKey,
            document.OriginalFileName,
            document.MimeType,
            document.SizeBytes,
            document.CreatedAtUtc);
    }
}
