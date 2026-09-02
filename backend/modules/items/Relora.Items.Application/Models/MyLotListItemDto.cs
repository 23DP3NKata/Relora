using Relora.Items.Domain.Enums;
using Relora.Shared.Domain.Enums;

namespace Relora.Items.Application.Models;

public sealed class MyLotListItemDto
{
    public Guid Id { get; init; }
    public Guid SellerId { get; init; }
    public string Title { get; init; } = default!;
    public string Description { get; init; } = default!;
    public decimal Price { get; init; }
    public string Currency { get; init; } = default!;
    public LotCategory Category { get; init; }
    public string CategoryName { get; init; } = default!;
    public LotGender Gender { get; init; }
    public string GenderName { get; init; } = default!;
    public LotSize Size { get; init; }
    public string SizeName { get; init; } = default!;
    public string Brand { get; init; } = default!;
    public LotCondition Condition { get; init; }
    public string ConditionName { get; init; } = default!;
    public string? Color { get; init; }
    public Guid CategoryId { get; init; }
    public Guid? RootCategoryId { get; init; }
    public string? CategorySlug { get; init; }
    public string? CategoryNameKey { get; init; }
    public string? RootCategorySlug { get; init; }
    public string? RootCategoryNameKey { get; init; }
    public LotDepartment Department { get; init; }
    public string DepartmentName { get; init; } = default!;
    public Guid PrimaryColorId { get; init; }
    public string? ModelName { get; init; }
    public int? AcquisitionYear { get; init; }
    public int? ProductionYear { get; init; }
    public bool IsVintage { get; init; }
    public string? VintageNotes { get; init; }
    public IReadOnlyList<LotMaterialDto> Materials { get; init; } = [];
    public IReadOnlyList<LotColorDto> SecondaryColors { get; init; } = [];
    public IReadOnlyList<LotMeasurementDto> Measurements { get; init; } = [];
    public string ProofStatus { get; init; } = "notProvided";
    public IReadOnlyList<LotProofDocumentDto> ProofDocuments { get; init; } = [];
    public LotStatus Status { get; init; }
    public string StatusName { get; init; } = default!;
    public Guid? AuctionId { get; init; }
    public DateTime? CreatedAt { get; init; }
    public decimal ShippingPrice { get; init; }
    public string ShippingCurrency { get; init; } = default!;
    public string ShippingOriginCountry { get; init; } = default!;
    public string ShipsToCountries { get; init; } = default!;
    public int ShippingHandlingDays { get; init; }
    public string? ModerationRejectionReason { get; init; }
    public Guid? ModerationReviewedByAdminId { get; init; }
    public DateTime? ModerationReviewedAtUtc { get; init; }
    public string? ModerationMessage { get; init; }
    public List<LotMediaDto> Media { get; init; } = new();
}
