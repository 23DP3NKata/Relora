using Relora.Support.Application.Commands;

using System.Security.Claims;

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
        // ja lietotājs ir ielogojies, piesaistām pieprasījumu viņa kontam
        Guid? userId = null;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var parsedUserId))
        {
            userId = parsedUserId;
        }

        await mediator.Send(new CreateSupportRequestCommand(command.Email, command.Category, command.Subject, command.Message, userId), cancellationToken);
        return Ok();
    }
}
