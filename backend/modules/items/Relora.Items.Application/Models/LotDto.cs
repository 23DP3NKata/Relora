using Relora.Items.Domain.Enums;
using Relora.Shared.Domain.Enums;

using static Relora.Items.Domain.Lot;

namespace Relora.Items.Application.Models;

/// <summary>
/// Represents the lot dto class.
/// </summary>
public sealed class LotDto
{
    /// <summary>
    /// Gets or sets the id used by this type.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the title used by this type.
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// Gets or sets the description used by this type.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the price used by this type.
    /// </summary>
    public decimal Price { get; set; }
    /// <summary>
    /// Gets or sets the currency used by this type.
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// Gets or sets the category used by this type.
    /// </summary>
    public LotCategory Category { get; set; }
    public string CategoryName { get; set; }
    /// <summary>
    /// Gets or sets the gender used by this type.
    /// </summary>
    public LotGender Gender { get; set; }
    public string GenderName { get; set; }
    /// <summary>
    /// Gets or sets the size used by this type.
    /// </summary>
    public LotSize Size { get; set; }
    public string SizeName { get; set; }
    /// <summary>
    /// Gets or sets the brand used by this type.
    /// </summary>
    public string Brand { get; set; }
    /// <summary>
    /// Gets or sets the condition used by this type.
    /// </summary>
    public LotCondition Condition { get; set; }
    public string ConditionName { get; set; }
    /// <summary>
    /// Gets or sets the color used by this type.
    /// </summary>
    public string? Color { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? RootCategoryId { get; set; }
    public string? CategorySlug { get; set; }
    public string? CategoryNameKey { get; set; }
    public string? RootCategorySlug { get; set; }
    public string? RootCategoryNameKey { get; set; }
    public LotDepartment Department { get; set; }
    public string DepartmentName { get; set; }
    public Guid PrimaryColorId { get; set; }
    public LotColorDto? PrimaryColor { get; set; }
    public IReadOnlyList<LotColorDto> SecondaryColors { get; set; } = [];
    public string? ModelName { get; set; }
    public int? AcquisitionYear { get; set; }
    public int? ProductionYear { get; set; }
    public bool IsVintage { get; set; }
    public string? VintageNotes { get; set; }
    public IReadOnlyList<LotMaterialDto> Materials { get; set; } = [];
    public IReadOnlyList<LotMeasurementDto> Measurements { get; set; } = [];
    public string ProofStatus { get; set; } = "notProvided";
    public IReadOnlyList<LotProofDocumentDto> ProofDocuments { get; set; } = [];

    public LotStatus Status { get; set; }
    public string StatusName { get; set; }

    public string Country { get; set; }
    public string City { get; set; }

    public string Age { get; set; }
    public string Style { get; set; }
    public decimal ShippingPrice { get; set; }
    public string ShippingCurrency { get; set; }
    public string ShippingOriginCountry { get; set; }
    public string ShipsToCountries { get; set; }
    public int ShippingHandlingDays { get; set; }

    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the media used by this type.
    /// </summary>
    public List<LotMediaDto> Media { get; set; } = new();

    public LotSellerDto Seller { get; set; }
}
