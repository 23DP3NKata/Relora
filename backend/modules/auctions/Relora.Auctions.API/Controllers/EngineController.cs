using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Relora.Auctions.Application.Commands;
using Relora.Identity.Infrastructure.Claims;
using Relora.Identity.Infrastructure.Constants;
using Relora.Shared.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Relora.Auctions.API.Controllers;

[ApiController]
[Route("api/auction")]
/// <summary>
/// Represents the engine controller class.
/// </summary>
public sealed class EngineController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="EngineController"/> class.
    /// </summary>
    /// <param name="mediator">Mediator.</param>
    public EngineController (IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("start/{auctionId}")]
    [Authorize]
    /// <summary>
    /// Starts auction.
    /// </summary>
    /// <param name="auctionId">Identifier of auction.</param>
    /// <param name="duration">Duration.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<IActionResult> StartAuction(Guid auctionId, AuctionDurationOption duration)
    {
        await _mediator.Send(
            new StartAuctionCommand(
                auctionId,
                duration,
                User.Claims.GetUserId(),
                User.IsInRole(Roles.Admin)));

        return Ok();
    }

    [HttpPost]
    [Route("stop/{auctionId}")]
    [Authorize]
    /// <summary>
    /// Stops auction.
    /// </summary>
    /// <param name="auctionId">Identifier of auction.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<IActionResult> StopAuction(Guid auctionId)
    {
        await _mediator.Send(
            new StopAuctionCommand(
                auctionId,
                User.Claims.GetUserId(),
                User.IsInRole(Roles.Admin)));

        return Ok();
    }
}
