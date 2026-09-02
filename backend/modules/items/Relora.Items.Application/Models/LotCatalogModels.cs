using Relora.Items.Domain.Enums;

namespace Relora.Items.Application.Models;

public sealed record LotMaterialInput(Guid MaterialId, decimal? Percentage, string? OtherName);

public sealed record LotMeasurementInput(string Key, decimal Value, string? Unit);

public sealed record ProofDocumentUploadInput(Guid UploadId);

public sealed record LookupOptionDto(
    Guid Id,
    string Code,
    string NameKey,
    int SortOrder,
    bool IsActive);

public sealed record CategoryDto(
    Guid Id,
    Guid? ParentId,
    string Slug,
    string NameKey,
    int SortOrder,
    bool IsActive,
    string MeasurementProfile,
    IReadOnlyList<LotDepartment> AllowedDepartments,
    IReadOnlyList<CategoryDto> Children);

public sealed record MeasurementDefinitionDto(
    Guid Id,
    string Profile,
    string Key,
    string LabelKey,
    bool IsRequired,
    int SortOrder,
    string Unit);

public sealed record SizeOptionDto(string Code, string NameKey, string CategoryGroup);

public sealed record LotFormLookupsDto(
    IReadOnlyList<LotDepartment> Departments,
    IReadOnlyList<CategoryDto> Categories,
    IReadOnlyList<LookupOptionDto> Materials,
    IReadOnlyList<LookupOptionDto> Colors,
    IReadOnlyList<LookupOptionDto> ProofDocumentTypes,
    IReadOnlyList<MeasurementDefinitionDto> MeasurementDefinitions,
    IReadOnlyList<SizeOptionDto> SizeOptions);

public sealed record LotMaterialDto(
    Guid MaterialId,
    string Code,
    string NameKey,
    decimal? Percentage,
    string? OtherName);

public sealed record LotColorDto(Guid ColorId, string Code, string NameKey);

public sealed record LotMeasurementDto(string Key, decimal Value, string Unit, string? LabelKey);

public sealed record LotProofDocumentDto(
    Guid Id,
    Guid DocumentTypeId,
    string TypeCode,
    string TypeNameKey,
    string OriginalFileName,
    string MimeType,
    long SizeBytes,
    DateTime CreatedAtUtc);
