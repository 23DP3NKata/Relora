using Relora.Admin.Application.Commands;
using Relora.Admin.Application.Handlers.Queries;
using Relora.Admin.Application.Queries;
using Relora.Admin.Application.Requests;
using Relora.Admin.Domain.Models;
using Relora.Identity.Infrastructure.Claims;
using Relora.Identity.Infrastructure.Constants;
using Relora.Items.Application.Models;
using Relora.Orders.Application.Commands;
using Relora.Orders.Application.Requests;
using Relora.Orders.Domain.Enums;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Relora.Admin.API;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = Roles.Admin)]
public sealed class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController (IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("dashboard")]
    public async Task<ActionResult<IReadOnlyList<AdminDashboardDto>>> GetDashboardData(CancellationToken cancellationToken)
    {
        var adminId = User.Claims.GetUserId();

        var result = await _mediator.Send(new GetAdminDashboardData(adminId), cancellationToken);

        return Ok(result);
    }

    [HttpPost("accept/{lotId:guid}")]
    public async Task<ActionResult> AcceptPendingLot(
        Guid lotId,
        CancellationToken cancellationToken)
    {
        var adminId = User.Claims.GetUserId();

        await _mediator.Send(new AcceptLotCommand(lotId, adminId), cancellationToken);
        return Ok();
    }

    [HttpPost("reject/{lotId:guid}")]
    public async Task<ActionResult> RejectPendingLot(
        Guid lotId,
        [FromBody] RejectLotRequest? request,
        CancellationToken cancellationToken)
    {
        var adminId = User.Claims.GetUserId();

        await _mediator.Send(new RejectLotCommand(lotId, adminId, request?.Reason), cancellationToken);
        return Ok();
    }

    [HttpGet]
    [Route("lots/pending")]
    public async Task<ActionResult<IReadOnlyList<PendingLotPreviewDto>>> GetListOfPendingLots(CancellationToken cancellationToken)
    {
        var adminId = User.Claims.GetUserId();
        var result = await _mediator.Send(new GetListOfPendingLots(adminId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("lots/pending/{lotId:guid}")]
    public async Task<ActionResult<PendingLotPreviewDetailsDto>> GetDetailsOfPendingLot(
        Guid lotId,
        CancellationToken cancellationToken)
    {
        var adminId = User.Claims.GetUserId();

        var result = await _mediator.Send(new GetPendingLotDetails(lotId, adminId), cancellationToken);

        if (result is null)
        {
            return NotFound();
        }    

        return Ok(result);
    }

    [HttpGet("orders/overdue-shipments")]
    public async Task<ActionResult<IReadOnlyList<OverdueShipmentOrderDto>>> GetOverdueShipmentOrders(
        CancellationToken cancellationToken)
    {
        var adminId = User.Claims.GetUserId();
        var result = await _mediator.Send(new GetOverdueShipmentOrders(adminId), cancellationToken);

        return Ok(result);
    }

    [HttpGet("orders/delivery-issues")]
    public async Task<ActionResult<IReadOnlyList<DeliveryIssueOrderDto>>> GetDeliveryIssueOrders(
        CancellationToken cancellationToken)
    {
        var adminId = User.Claims.GetUserId();
        var result = await _mediator.Send(new GetDeliveryIssueOrders(adminId), cancellationToken);

        return Ok(result);
    }

    [HttpGet("disputes")]
    public async Task<ActionResult<IReadOnlyList<AdminOrderDisputeDto>>> GetOrderDisputes(
        CancellationToken cancellationToken)
    {
        var adminId = User.Claims.GetUserId();
        var result = await _mediator.Send(new GetOrderDisputes(adminId), cancellationToken);

        return Ok(result);
    }

    [HttpPost("disputes/{disputeId:guid}/resolve")]
    public async Task<ActionResult> ResolveOrderDispute(
        Guid disputeId,
        [FromBody] ResolveOrderDisputeRequest request,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<OrderDisputeDecision>(request.Decision?.Trim(), ignoreCase: true, out var decision))
        {
            return BadRequest(new
            {
                message = "Invalid dispute decision.",
                allowedDecisions = Enum.GetNames<OrderDisputeDecision>()
            });
        }

        var adminId = User.Claims.GetUserId();
        await _mediator.Send(
            new ResolveOrderDisputeCommand(disputeId, adminId, decision, request.Reason),
            cancellationToken);

        return Ok();
    }
}
