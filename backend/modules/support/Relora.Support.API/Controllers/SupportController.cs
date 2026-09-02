using Relora.Support.Application.Commands;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Relora.Support.API.Controllers;

[ApiController]
[Route("api/support")]
public sealed class SupportController(IMediator mediator) : ControllerBase
{
    [HttpPost("requests")]
    [EnableRateLimiting("SupportPolicy")]
    public async Task<ActionResult> Create
    (
        [FromBody] CreateSupportRequestCommand command,
        CancellationToken cancellationToken
    )
    {
        var id = await mediator.Send(new CreateSupportRequestCommand(command.Email, command.Category, command.Subject, command.Message), cancellationToken);
        return Ok();
    }
}
