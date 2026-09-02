using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using MediatR;

using Relora.Bids.Application.Models;
using Relora.Bids.Application.Queries;
using Relora.Bids.Application.Commands;
using Microsoft.AspNetCore.RateLimiting;
using Relora.Identity.Infrastructure.Claims;
using Relora.Identity.Infrastructure.Constants;

namespace Relora.Bids.API.Controllers;

[ApiController]
[Route("api/bids")]
/// <summary>
/// Represents the bids controller class.
/// </summary>
public sealed class BidsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="BidsController"/> class.
    /// </summary>
    /// <param name="mediator">Mediator.</param>
    public BidsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("{auctionId}")]
    [AllowAnonymous]
    /// <summary>
    /// Gets bids by auction.
    /// </summary>
    /// <param name="auctionId">Identifier of auction.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<ActionResult<IReadOnlyList<BidsByAuctionDto>>> GetBidsByAuction(Guid auctionId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetBidsByAuctionQuery(
                auctionId,
                User.Claims.TryGetUserId(),
                User.IsInRole(Roles.Admin)),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Route("place/{auctionId}")]
    [Authorize]
    [EnableRateLimiting("BidsPolicy")]
    /// <summary>
    /// Places bid.
    /// </summary>
    /// <param name="auctionId">Identifier of auction.</param>
    /// <param name="request">Input data for the operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<IActionResult> PlaceBid([FromRoute] Guid auctionId, [FromBody] PlaceBidCommand request, CancellationToken cancellationToken)
    {
        var bidderId = User.Claims.GetUserId();

        await _mediator.Send(new PlaceBidCommand(auctionId, bidderId, request.Amount, request.Currency), cancellationToken);

        return Ok();
    }
}
