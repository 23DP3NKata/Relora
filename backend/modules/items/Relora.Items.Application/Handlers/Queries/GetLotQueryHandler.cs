using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Relora.Identity.Application.Interfaces;
using Relora.Items.Application.Interfaces;
using Relora.Items.Application.Models;
using Relora.Items.Application.Queries;
using Relora.Items.Application.Services;
using Relora.Shared.Domain.Enums;
using Relora.Shared.Infrastructure.Media;

using MediatR;

using Microsoft.Extensions.Options;

namespace Relora.Items.Application.Handlers.Queries;
/// <summary>
/// Represents the get lot query handler class.
/// </summary>
public sealed class GetLotQueryHandler(
    ILotRepository lotRepository,
    ILotCatalogRepository catalogRepository,
    IUserRepository userRepository,
    IOptions<MediaOptions> mediaOptions) : IRequestHandler<GetLotQuery, LotDto>
{
    private static readonly LotStatus[] PublicLotStatuses =
    [
        LotStatus.Published,
        LotStatus.Listed,
        LotStatus.Sold
    ];

    private readonly ILotRepository _lotRepository = lotRepository;
    private readonly ILotCatalogRepository _catalogRepository = catalogRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly string _publicBaseUrl = mediaOptions.Value.PublicBaseUrl;

    /// <summary>
    /// Handles the operation.
    /// </summary>
    /// <param name="request">Input data for the operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<LotDto> Handle(GetLotQuery request, CancellationToken cancellationToken)
    {
        var lot = await _lotRepository.GetLotById(request.lotId, cancellationToken);

        if (lot == null)
        {
            throw new KeyNotFoundException($"Lot is null");
        }

        if (!request.ViewerIsAdmin &&
            (!request.ViewerUserId.HasValue || lot.SellerId != request.ViewerUserId.Value) &&
            !PublicLotStatuses.Contains(lot.Status))
        {
            throw new KeyNotFoundException($"Lot with id {request.lotId} not found.");
        }

        var seller = await _userRepository.GetUserByIdAsync(lot.SellerId);

        if (seller is null)
        {
            throw new KeyNotFoundException("Seller not found.");
        }

        var catalogContext = await CreateCatalogContextAsync(cancellationToken);
        var canShowPrivateProofDocuments = lot.CanShowProofMetadata(request.ViewerUserId, request.ViewerIsAdmin);

        return LotReadModelMapper.ToDetails(
            lot,
            catalogContext,
            _publicBaseUrl,
            canShowPrivateProofDocuments,
            new LotSellerDto
            {
                Id = seller.Id,
                Name = seller.Name,
                Username = seller.UserName,
            });
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
