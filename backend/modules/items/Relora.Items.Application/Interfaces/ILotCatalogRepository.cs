using Relora.Items.Domain;
using Relora.Items.Domain.Enums;

namespace Relora.Items.Application.Interfaces;

public interface ILotCatalogRepository
{
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken);
    Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Category?> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken);
    Task<IReadOnlyList<Category>> GetCategoriesBySlugsAsync(IReadOnlyCollection<string> slugs, CancellationToken cancellationToken);
    Task<IReadOnlyList<Material>> GetMaterialsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Material>> GetMaterialsByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken);
    Task<Material?> GetMaterialByCodeAsync(string code, CancellationToken cancellationToken);
    Task<IReadOnlyList<ItemColor>> GetColorsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ItemColor>> GetColorsByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken);
    Task<ItemColor?> GetColorByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ItemColor?> GetColorByCodeAsync(string code, CancellationToken cancellationToken);
    Task<IReadOnlyList<MeasurementDefinition>> GetMeasurementDefinitionsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<MeasurementDefinition>> GetMeasurementDefinitionsByProfileAsync(string profile, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProofDocumentType>> GetProofDocumentTypesAsync(CancellationToken cancellationToken);
    Task<ProofDocumentType?> GetProofDocumentTypeByIdAsync(Guid id, CancellationToken cancellationToken);
}
