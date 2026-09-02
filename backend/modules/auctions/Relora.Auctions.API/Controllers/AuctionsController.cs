using Relora.Auctions.Application.Commands;
using Relora.Auctions.Application.Models;
using Relora.Auctions.Application.Queries;

using Relora.Identity.Infrastructure.Claims;
using Relora.Identity.Infrastructure.Constants;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Relora.Auctions.API.Controllers;

public sealed record CreateAuctionRequest(Guid LotId);

[ApiController]
[Route("api/auctions")]
/// <summary>
/// Represents the auctions controller class.
/// </summary>
public sealed class AuctionsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuctionsController"/> class.
    /// </summary>
    /// <param name="mediator">Mediator.</param>
    public AuctionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{auctionId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<AuctionDetailsDto>> GetAuctionDetails(
        Guid auctionId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAuctionDetailsQuery(
                auctionId,
                User.Claims.TryGetUserId(),
                User.IsInRole(Roles.Admin)),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("lot/{lotId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<AuctionDetailsDto>> GetAuctionByLot(
        Guid lotId,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(
                new GetAuctionByLotQuery(
                    lotId,
                    User.Claims.TryGetUserId(),
                    User.IsInRole(Roles.Admin)),
                cancellationToken);

            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Auction not found." });
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<AuctionListItemDto>>> GetAuctionList(
        [FromQuery] GetAuctionsListQuery query,
        CancellationToken cancellationToken)
    {
        var request = query with
        {
            ViewerUserId = User.Claims.TryGetUserId(),
            ViewerIsAdmin = User.IsInRole(Roles.Admin)
        };

        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Route("create")]
    [Authorize]
    /// <summary>
    /// Creates auction.
    /// </summary>
    /// <param name="request">Input data for the operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<ActionResult> CreateAuction([FromBody] CreateAuctionRequest request, CancellationToken cancellationToken)
    {
        var auctionId = await _mediator.Send(
            new CreateAuctionCommand(
                request.LotId,
                User.Claims.GetUserId(),
                User.IsInRole(Roles.Admin)),
            cancellationToken);

        return Ok(auctionId);
    }
}
