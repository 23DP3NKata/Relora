using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Relora.Orders.Application.Queries;
using Relora.Orders.Application.Commands;
using Relora.Orders.Application.Requests;
using Relora.Orders.Domain.Enums;
using Relora.Identity.Infrastructure.Claims;

namespace Relora.Orders.API.Controllers;

[ApiController]
[Route("api/orders")]
/// <summary>
/// Represents the orders controller class.
/// </summary>
public sealed class OrdersController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [Authorize]
    [Route("my")]
    /// <summary>
    /// Gets orders for user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<ActionResult> GetOrdersForUser(CancellationToken cancellationToken)
    {
        var userId = User.Claims.GetUserId();
        var result = await _mediator.Send(new GetOrdersByUserIdQuery(userId),cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [Route("details/{orderId:guid}")]
    [Authorize]
    /// <summary>
    /// Gets order details.
    /// </summary>
    /// <param name="orderId">Identifier of order.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<ActionResult> GetOrderDetails(Guid orderId, CancellationToken cancellationToken)
    {
        var userId = User.Claims.GetUserId();
        var result = await _mediator.Send(new GetOrderDetailsQuery(orderId, userId),cancellationToken);
        return Ok(result);
    }

    [HttpPost("{orderId:guid}/shipment")]
    [Authorize]
    public async Task<ActionResult> MarkOrderShipped(
        Guid orderId,
        [FromBody] MarkOrderShippedRequest request,
        CancellationToken cancellationToken)
    {
        var sellerId = User.Claims.GetUserId();

        await _mediator.Send(
            new MarkOrderShippedCommand(orderId, sellerId, request.CarrierName, request.TrackingNumber),
            cancellationToken);

        return Ok();
    }

    [HttpPost("{orderId:guid}/received")]
    [Authorize]
    public async Task<ActionResult> ConfirmOrderReceived(Guid orderId, CancellationToken cancellationToken)
    {
        var buyerId = User.Claims.GetUserId();

        await _mediator.Send(new ConfirmOrderReceivedCommand(orderId, buyerId), cancellationToken);

        return Ok();
    }

    [HttpPost("{orderId:guid}/problem")]
    [Authorize]
    public async Task<ActionResult> OpenOrderDispute(
        Guid orderId,
        [FromBody] OpenOrderDisputeRequest request,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<OrderDisputeReason>(request.Reason?.Trim(), ignoreCase: true, out var reason))
        {
            return BadRequest(new
            {
                message = "Invalid dispute reason.",
                allowedReasons = Enum.GetNames<OrderDisputeReason>()
            });
        }

        var buyerId = User.Claims.GetUserId();
        var disputeId = await _mediator.Send(
            new OpenOrderDisputeCommand(
                orderId,
                buyerId,
                reason,
                request.Description,
                request.EvidenceKeys),
            cancellationToken);

        return Ok(new { disputeId });
    }

    [HttpPost("{orderId:guid}/not-delivered")]
    [Authorize]
    public async Task<ActionResult> ReportOrderNotDelivered(
        Guid orderId,
        [FromBody] ReportOrderNotDeliveredRequest? request,
        CancellationToken cancellationToken)
    {
        var buyerId = User.Claims.GetUserId();

        await _mediator.Send(
            new ReportOrderNotDeliveredCommand(orderId, buyerId, request?.Reason),
            cancellationToken);

        return Ok();
    }
}
