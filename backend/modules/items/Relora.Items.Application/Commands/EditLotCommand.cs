using Relora.Items.Application.Models;
using Relora.Items.Domain.Enums;
using Relora.Shared.Domain.ValueObjects;

using MediatR;

namespace Relora.Items.Application.Commands;

/// <summary>
/// Represents the edit lot command record.
/// </summary>
public sealed record EditLotCommand(
    Guid id,
    Guid sellerId,
    string title,
    string description,
    Money price,
    LotSize size,
    string brand,
    LotCategory category,
    LotGender gender,
    LotCondition condition,
    string? color,
    string? country,
    string? city,
    Guid? categoryId,
    LotDepartment? department,
    Guid? primaryColorId,
    string? modelName,
    int? acquisitionYear,
    int? productionYear,
    bool isVintage,
    string? vintageNotes,
    string age,
    string style,
    decimal shippingPrice,
    string shippingCurrency,
    string shippingOriginCountry,
    string shipsToCountries,
    int shippingHandlingDays,
    IReadOnlyList<string>? photoKeys,
    string? coverPhotoKey,
    IReadOnlyList<LotMaterialInput>? materials,
    IReadOnlyList<LotMeasurementInput>? measurements,
    IReadOnlyList<ProofDocumentUploadInput>? proofDocuments
) : IRequest;
