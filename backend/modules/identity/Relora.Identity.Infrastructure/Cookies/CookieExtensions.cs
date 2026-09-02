using Microsoft.AspNetCore.Http;
using Relora.Identity.Application.Interfaces;

namespace Relora.Identity.Infrastructure.Cookies;

public class CookieFactory : ICookieFactory
{
    public void SetAccessTokenCookie(HttpResponse response, string token)
    {
        // Adds a cookie to the HTTP response.
        // The browser will store it and automatically send it back
        // on future requests if the cookie rules allow it.
        response.Cookies.Append("access_token", token, new CookieOptions
        {
            // Prevents JavaScript from accessing the cookie via document.cookie.
            // This is important for security because it helps protect the token
            // from being stolen through XSS attacks.
            HttpOnly = true,

            // Ensures the cookie is sent only over HTTPS connections.
            // If the site is opened over plain HTTP, this cookie will not be sent.
            Secure = true,

            // Controls when the cookie is sent in cross-site requests.
            // Lax is a balanced default: safer than None, but less strict than Strict.
            SameSite = SameSiteMode.Lax,

            // Sets the exact expiration date and time of the cookie.
            // After this moment, the browser should remove it.
            Expires = DateTimeOffset.UtcNow.AddMinutes(30),

            // Makes the cookie available for the entire website.
            // It will be sent for all routes such as /api, /profile, /orders, etc.
            Path = "/"
        });
    }

    public void SetRefreshTokenCookie(HttpResponse response, string token)
    {
        response.Cookies.Append("refresh_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
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
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            Path = "/"
        });
    }

    public void DeleteAccessTokenCookie(HttpResponse response)
    {
        // Deletes the cookie with the given name.
        // Path should match the original cookie path,
        // otherwise the browser may not remove the correct cookie.
        response.Cookies.Delete("access_token", new CookieOptions
        {
            Path = "/",
            Secure = true,
            SameSite = SameSiteMode.Lax,
            HttpOnly = true
        });
    }

    public void DeleteRefreshTokenCookie(HttpResponse response)
    {
        response.Cookies.Delete("refresh_token", new CookieOptions
        {
            Path = "/",
            Secure = true,
            SameSite = SameSiteMode.Lax,
            HttpOnly = true
        });
    }
}