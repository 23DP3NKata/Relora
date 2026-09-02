using Microsoft.AspNetCore.Http;

namespace Relora.Identity.Application.Interfaces;

public interface ICookieFactory
{
    void SetAccessTokenCookie(HttpResponse httpResponse, string token);
    void SetRefreshTokenCookie(HttpResponse response, string token);
    void DeleteAccessTokenCookie(HttpResponse response);
    void DeleteRefreshTokenCookie(HttpResponse response);
    void SetUserPreferenceCookie(HttpResponse response, string preference);
}
