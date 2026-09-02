using Relora.Items.Application.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Relora.Items.API.Controllers;

[ApiController]
[Route("api/lookups")]
public sealed class LookupsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("lot-form")]
    public async Task<ActionResult> GetLotFormLookups(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetLotFormLookupsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("categories/{categoryId:guid}/measurements")]
    public async Task<ActionResult> GetMeasurementSchema(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMeasurementSchemaQuery(categoryId), cancellationToken);
        return Ok(result);
    }
}
