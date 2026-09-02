using Relora.Items.Application.Interfaces;
using Relora.Items.Application.Models;
using Relora.Items.Application.Queries;
using Relora.Items.Application.Services;
using Relora.Shared.Domain.Enums;
using Relora.Shared.Infrastructure.Media;

using MediatR;

using Microsoft.Extensions.Options;

namespace Relora.Items.Application.Handlers.Queries;

public sealed class GetLotsByUserIdQueryHandler(
    ILotRepository lotRepository,
    ILotCatalogRepository catalogRepository,
    IOptions<MediaOptions> mediaOptions)
    : IRequestHandler<GetLotsByUserIdQuery, IReadOnlyList<MyLotListItemDto>>
{
    private readonly ILotRepository _lotRepository = lotRepository;
    private readonly ILotCatalogRepository _catalogRepository = catalogRepository;
    private readonly string _publicBaseUrl = mediaOptions.Value.PublicBaseUrl;

    public async Task<IReadOnlyList<MyLotListItemDto>> Handle(GetLotsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var lots = await _lotRepository.GetLotsBySellerIdAsync(request.UserId, cancellationToken);
        var catalogContext = await CreateCatalogContextAsync(cancellationToken);

        return lots
            .Select(lot => LotReadModelMapper.ToMyLot(lot, catalogContext, _publicBaseUrl))
            .ToList();
    }

    private async Task<LotCatalogReadContext> CreateCatalogContextAsync(CancellationToken cancellationToken)
    {
        var categories = await _catalogRepository.GetCategoriesAsync(cancellationToken);
        var colors = await _catalogRepository.GetColorsAsync(cancellationToken);
        var materials = await _catalogRepository.GetMaterialsAsync(cancellationToken);
        var proofTypes = await _catalogRepository.GetProofDocumentTypesAsync(cancellationToken);
        var measurements = await _catalogRepository.GetMeasurementDefinitionsAsync(cancellationToken);

        return LotReadModelMapper.CreateContext(categories, colors, materials, proofTypes, measurements);
    }
}
