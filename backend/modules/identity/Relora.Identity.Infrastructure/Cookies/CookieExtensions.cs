using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Relora.Identity.Application.Interfaces;

namespace Relora.Identity.Infrastructure.Cookies;

public class CookieFactory(IHostEnvironment environment) : ICookieFactory
{
    private readonly SameSiteMode _sameSite = environment.IsDevelopment()
        ? SameSiteMode.None
        : SameSiteMode.Lax;

    public void SetAccessTokenCookie(HttpResponse response, string token)
    {
        response.Cookies.Append("access_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = _sameSite,
            Expires = DateTimeOffset.UtcNow.AddMinutes(30),
            Path = "/"
        });
    }

    public void SetRefreshTokenCookie(HttpResponse response, string token)
    {
        response.Cookies.Append("refresh_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = _sameSite,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/"
        });
    }

    public void SetUserPreferenceCookie(HttpResponse response, string preference)
    {
        response.Cookies.Append("user_preference", preference, new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = _sameSite,
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            Path = "/"
        });
    }

    public void DeleteAccessTokenCookie(HttpResponse response)
    {
        response.Cookies.Delete("access_token", new CookieOptions
        {
            Path = "/",
            Secure = true,
            SameSite = _sameSite,
            HttpOnly = true
        });
    }

    public void DeleteRefreshTokenCookie(HttpResponse response)
    {
        response.Cookies.Delete("refresh_token", new CookieOptions
        {
            Path = "/",
            Secure = true,
            SameSite = _sameSite,
            HttpOnly = true
        });
    }
}
