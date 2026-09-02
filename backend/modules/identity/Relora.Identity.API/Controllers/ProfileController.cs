using Relora.Identity.Application.Models;
using Relora.Identity.Application.Queries;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using MediatR;

namespace Relora.Identity.API.Controllers;

[ApiController]
[Route("api/profile")]
public sealed class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{username}")]
    [AllowAnonymous]
    public async Task<ActionResult<UserProfileDto>> GetUserProfile(string username, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetUserProfileQuery(username), cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
