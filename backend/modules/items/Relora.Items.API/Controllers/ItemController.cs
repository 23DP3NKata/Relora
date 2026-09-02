using System.Security.Claims;

using Relora.Identity.Infrastructure.Claims;
using Relora.Identity.Infrastructure.Constants;
using Relora.Items.Application.Commands;
using Relora.Items.Application.Queries;
using Relora.Items.Application.Requests;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Org.BouncyCastle.Asn1.Ocsp;

namespace Relora.Items.API.Controllers;

[ApiController]
[Route("api/items")]
/// <summary>
/// Represents the item controller class.
/// </summary>
public sealed class ItemController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemController"/> class.
    /// </summary>
    /// <param name="mediator">Mediator.</param>
    public ItemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    [Authorize]
    /// <summary>
    /// Creates lot.
    /// </summary>
    /// <param name="command">Input data for the operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<ActionResult<Guid>> CreateLot([FromBody] CreateLotRequest command, CancellationToken cancellationToken)
    {
        var sellerId = User.Claims.GetUserId();

        var request = new CreateLotCommand(
            sellerId,
            command.Title,
            command.Description,
            command.Amount,
            command.Currency,
            command.Category,
            command.Gender,
            command.Size,
            command.Brand,
            command.Condition,
            command.Color,
            command.Country,
            command.City,
            command.CategoryId,
            command.Department,
            command.PrimaryColorId,
            command.ModelName,
            command.AcquisitionYear,
            command.ProductionYear,
            command.IsVintage,
            command.VintageNotes,
            command.Age,
            command.Style,
            command.ShippingPrice,
            command.ShippingCurrency,
            command.ShippingOriginCountry,
            command.ShipsToCountries,
            command.ShippingHandlingDays,
            command.PhotoKeys,
            command.CoverPhotoKey,
            command.Materials,
            command.Measurements,
            command.ProofDocuments
        );

        var lotId = await _mediator.Send(request, cancellationToken);
        return Ok(lotId);
    }

    [HttpPatch]
    [Route("{lotId}/update")]
    [Authorize]
    /// <summary>
    /// Updates lot.
    /// </summary>
    /// <param name="lotId">Identifier of lot.</param>
    /// <param name="command">Input data for the operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<ActionResult> UpdateLot(Guid lotId, [FromBody] EditLotRequest request, CancellationToken cancellationToken)
    {
        var sellerId = User.Claims.GetUserId();
        var command = new EditLotCommand(
            lotId,
            sellerId,
            request.Title,
            request.Description,
            new Relora.Shared.Domain.ValueObjects.Money(request.Amount, request.Currency),
            request.Size,
            request.Brand,
            request.Category,
            request.Gender,
            request.Condition,
            request.Color,
            request.Country,
            request.City,
            request.CategoryId,
            request.Department,
            request.PrimaryColorId,
            request.ModelName,
            request.AcquisitionYear,
            request.ProductionYear,
            request.IsVintage,
            request.VintageNotes,
            request.Age,
            request.Style,
            request.ShippingPrice,
            request.ShippingCurrency,
            request.ShippingOriginCountry,
            request.ShipsToCountries,
            request.ShippingHandlingDays,
            request.PhotoKeys,
            request.CoverPhotoKey,
            request.Materials,
            request.Measurements,
            request.ProofDocuments);
        await _mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpDelete]
    [Route("{lotId}/delete")]
    [Authorize]
    /// <summary>
    /// Deletes lot.
    /// </summary>
    /// <param name="lotId">Identifier of lot.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<ActionResult> DeleteLot(Guid lotId, CancellationToken cancellationToken)
    {
        var userId = User.Claims.GetUserId();

        await _mediator.Send(new DeleteLotCommand(lotId, userId), cancellationToken);
        return Ok();
    }

    [HttpPost]
    [Route("{lotId}/publish")]
    [Authorize]
    /// <summary>
    /// Publishes lot.
    /// </summary>
    /// <param name="lotId">Identifier of lot.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<ActionResult> SubmitForReviewLot(Guid lotId, CancellationToken cancellationToken)
    {
        var userId = User.Claims.GetUserId();

        await _mediator.Send(new PublishLotCommand(lotId, userId), cancellationToken);
        return Ok();
    }

    [HttpGet]
    [Route("{lotId:guid}")]
    /// <summary>
    /// Gets lot.
    /// </summary>
    /// <param name="lotId">Identifier of lot.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<ActionResult> GetLot(Guid lotId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetLotQuery(
                lotId,
                User.Claims.TryGetUserId(),
                User.IsInRole(Roles.Admin)),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    [Route("my")]
    [Authorize]
    public async Task<ActionResult> GetMyLots(CancellationToken cancellationToken)
    {
        var userId = User.Claims.GetUserId();
        var result = await _mediator.Send(new GetLotsByUserIdQuery(userId), cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    /// <summary>
    /// Gets lots filtered list.
    /// </summary>
    /// <param name="query">Input data for the operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<ActionResult> GetLotsFilteredList([FromQuery] GetLotsListQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
