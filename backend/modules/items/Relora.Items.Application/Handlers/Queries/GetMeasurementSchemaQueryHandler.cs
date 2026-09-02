using Relora.Items.Application.Interfaces;
using Relora.Items.Application.Models;
using Relora.Items.Application.Queries;

using MediatR;

namespace Relora.Items.Application.Handlers.Queries;

public sealed class GetMeasurementSchemaQueryHandler(ILotCatalogRepository catalogRepository)
    : IRequestHandler<GetMeasurementSchemaQuery, IReadOnlyList<MeasurementDefinitionDto>>
{
    private readonly ILotCatalogRepository _catalogRepository = catalogRepository;

    public async Task<IReadOnlyList<MeasurementDefinitionDto>> Handle(
        GetMeasurementSchemaQuery request,
        CancellationToken cancellationToken)
    {
        var category = await _catalogRepository.GetCategoryByIdAsync(request.CategoryId, cancellationToken);

        if (category is null || !category.IsActive)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        var definitions = await _catalogRepository.GetMeasurementDefinitionsByProfileAsync(
            category.MeasurementProfile,
            cancellationToken);

        return definitions
            .Select(definition => new MeasurementDefinitionDto(
                definition.Id,
                definition.Profile,
                definition.Key,
                definition.LabelKey,
                definition.IsRequired,
                definition.SortOrder,
                definition.Unit))
            .ToList();
    }
}
