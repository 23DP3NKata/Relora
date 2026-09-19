using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Relora.Identity.Application.Interfaces;
using Relora.Identity.Infrastructure.Claims;

namespace Relora.Identity.API.Controllers;

[ApiController]
[Authorize]
[Route("api/settings")]
public sealed class SettingsController(
    IUserRepository users,
    IPasswordHasher passwords,
    ITokenProvider tokens,
    ICookieFactory cookies) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Get()
    {
        var user = await users.GetUserByIdAsync(User.Claims.GetUserId());
        if (user is null) return Unauthorized();
        return Ok(new { userId = user.Id, name = user.Name, username = user.UserName, email = user.Email });
    }

    [HttpPut("profile")]
    public async Task<ActionResult> UpdateProfile(ProfileSettings request)
    {
        var name = request.Name.Trim();
        var username = request.Username.Trim();
        if (name.Length == 0 || name.Any(char.IsControl))
            ModelState.AddModelError("name", "invalidName");
        if (username.Length < 3 || username.Any(character => char.IsWhiteSpace(character) || char.IsControl(character) || "/?#%\\".Contains(character)))
            ModelState.AddModelError("username", "invalidUsername");
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var user = await users.GetUserByIdAsync(User.Claims.GetUserId());
        if (user is null) return Unauthorized();
        var existing = await users.GetUserByUsernameAsync(username);
        if (existing is not null && existing.Id != user.Id)
        {
            ModelState.AddModelError("username", "usernameTaken");
            return ValidationProblem(ModelState);
        }

        user.UpdateProfile(name, username);
        await users.UpdateUserAsync(user);
        cookies.SetAccessTokenCookie(Response, tokens.GenerateAccessToken(user));
        return Ok(new { userId = user.Id, name = user.Name, username = user.UserName, email = user.Email });
    }

    [HttpPut("password")]
    [EnableRateLimiting("AuthLoginPolicy")]
    public async Task<ActionResult> ChangePassword(PasswordSettings request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.NewPassword) || Encoding.UTF8.GetByteCount(request.NewPassword) > 72)
            ModelState.AddModelError("newPassword", "invalidPassword");
        if (request.NewPassword != request.ConfirmPassword)
            ModelState.AddModelError("confirmPassword", "passwordMismatch");
        if (request.NewPassword == request.CurrentPassword)
            ModelState.AddModelError("newPassword", "samePassword");
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var user = await users.GetUserByIdAsync(User.Claims.GetUserId());
        if (user is null) return Unauthorized();
        if (!passwords.VerifyPassword(user.PasswordHash, request.CurrentPassword))
        {
            ModelState.AddModelError("currentPassword", "incorrectPassword");
            return ValidationProblem(ModelState);
        }

        await users.ChangePasswordAsync(user, passwords.HashPassword(request.NewPassword), cancellationToken);
        cookies.DeleteAccessTokenCookie(Response);
        cookies.DeleteRefreshTokenCookie(Response);
        return NoContent();
    }
}

public sealed record ProfileSettings(
    [property: Required, StringLength(100)] string Name,
    [property: Required, StringLength(20, MinimumLength = 3)] string Username);

public sealed record PasswordSettings(
    [property: Required, StringLength(256)] string CurrentPassword,
    [property: Required, StringLength(72, MinimumLength = 8)] string NewPassword,
    [property: Required, StringLength(72)] string ConfirmPassword);
