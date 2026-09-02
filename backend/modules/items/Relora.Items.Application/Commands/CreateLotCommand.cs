using Relora.Items.Application.Models;
using Relora.Items.Domain.Enums;

using MediatR;

namespace Relora.Items.Application.Commands;

/// <summary>
/// Represents the create lot command record.
/// </summary>
public sealed record CreateLotCommand(
    Guid SellerId,
    string Title,
    string Description,
    decimal Amount,
    string Currency,
    LotCategory Category,
    LotGender Gender,
    LotSize Size,
    string Brand,
    LotCondition Condition,
    string? Color,
    string? Country,
    string? City,
    Guid? CategoryId,
    LotDepartment? Department,
    Guid? PrimaryColorId,
    string? ModelName,
    int? AcquisitionYear,
    int? ProductionYear,
    bool IsVintage,
    string? VintageNotes,
    string Age,
    string Style,
    decimal ShippingPrice,
    string ShippingCurrency,
    string ShippingOriginCountry,
    string ShipsToCountries,
    int ShippingHandlingDays,
    IReadOnlyList<string>? PhotoKeys,
    string? CoverPhotoKey,
    IReadOnlyList<LotMaterialInput>? Materials,
    IReadOnlyList<LotMeasurementInput>? Measurements,
    IReadOnlyList<ProofDocumentUploadInput>? ProofDocuments
) : IRequest<Guid>;
