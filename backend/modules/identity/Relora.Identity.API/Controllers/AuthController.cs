using Relora.Identity.Application.Commands;
using Relora.Identity.Application.Interfaces;
using Relora.Identity.Application.Models;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Relora.Identity.Infrastructure.Claims;
using Microsoft.AspNetCore.Http;

namespace Relora.Identity.API.Controllers;

[ApiController]
[Route("api/auth")]
/// <summary>
/// Represents the auth controller class.
/// </summary>
public sealed class AuthController(IMediator mediator, ICookieFactory cookieFactory) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ICookieFactory _cookieFactory = cookieFactory;

    [HttpPost("register")]
    [AllowAnonymous]
    [EnableRateLimiting("AuthRegisterPolicy")]
    /// <summary>
    /// Performs the register operation.
    /// </summary>
    /// <param name="command">Input data for the operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<ActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        _cookieFactory.SetAccessTokenCookie(Response, result.AccessToken);
        _cookieFactory.SetRefreshTokenCookie(Response, result.RefreshToken);

        return Ok();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("AuthLoginPolicy")]
    /// <summary>
    /// Performs the login operation.
    /// </summary>
    /// <param name="command">Input data for the operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<ActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        AuthResult result;

        try
        {
            result = await _mediator.Send(command, cancellationToken);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(BuildInvalidCredentialsProblem());
        }

        _cookieFactory.SetAccessTokenCookie(Response, result.AccessToken);
        _cookieFactory.SetRefreshTokenCookie(Response, result.RefreshToken);

        return Ok();
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refresh_token"];

        await _mediator.Send(new LogoutCommand(refreshToken), cancellationToken);

        _cookieFactory.DeleteAccessTokenCookie(Response);
        _cookieFactory.DeleteRefreshTokenCookie(Response);

        return Ok();
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("AuthRefreshPolicy")]
    public async Task<ActionResult> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refresh_token"];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized();
        }

        var result = await _mediator.Send(new RefreshTokenCommand(refreshToken), cancellationToken);

        _cookieFactory.SetAccessTokenCookie(Response, result.AccessToken);
        _cookieFactory.SetRefreshTokenCookie(Response, result.RefreshToken);

        return Ok();
    }

    [HttpGet("me")]
    [Authorize]
    [EnableRateLimiting("AuthMePolicy")]
    public async Task<ActionResult<AuthResult>> CurrentUser(CancellationToken cancellationToken)
    {
        var user = User.Claims.GetUserId();

        var result = await _mediator.Send(new CurrentUserCommand(user), cancellationToken);

        return Ok(result);
    }

    [HttpPost("set-preference")]
    [EnableRateLimiting("AuthMePolicy")]
    public async Task<ActionResult> SetPreference([FromBody] UserPreferencesCommand command, CancellationToken cancellationToken)
    {
        if (command.preference is not ("men" or "women") || string.IsNullOrWhiteSpace(command.preference))
        {
            return BadRequest("Invalid preference");
        }

        _cookieFactory.SetUserPreferenceCookie(Response, command.preference);

        return Ok();
    }

    private ProblemDetails BuildInvalidCredentialsProblem()
    {
        const int statusCode = StatusCodes.Status401Unauthorized;
        const string message = "Invalid email or password.";

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = "Unauthorized",
            Type = $"https://httpstatuses.com/{statusCode}",
            Detail = message,
            Instance = Request.Path
        };

        problemDetails.Extensions["message"] = message;
        problemDetails.Extensions["traceId"] = HttpContext.TraceIdentifier;

        return problemDetails;
    }
}
