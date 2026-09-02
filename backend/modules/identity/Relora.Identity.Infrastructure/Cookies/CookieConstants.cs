using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relora.Identity.Infrastructure.Cookies;
public static class CookieConstants
{
    public const int AccessTokenExpirationDays = 7;
    public const string AccessTokenCookieName = "token";
}
