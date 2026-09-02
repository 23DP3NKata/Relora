using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Relora.Identity.Application.Interfaces;
using Relora.Identity.Domain;
using Relora.Identity.Infrastructure.Configuration;
using Relora.Identity.Infrastructure.Constants;

using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Relora.Identity.Infrastructure.Token;

/// <summary>
/// Represents the token provider class.
/// </summary>
internal sealed class TokenProvider(IOptions<JwtOptions> options, IRefreshTokenRepository refreshTokenRepository) : ITokenProvider
{
    private readonly JwtOptions _jwtOptions = options.Value;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

    /// <summary>
    /// Performs the generate operation.
    /// </summary>
    /// <param name="user">User.</param>
    /// <returns>The operation result.</returns>
    public string GenerateAccessToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("username", user.UserName),
                new Claim(ClaimTypes.Role, user.IsAdmin ? Roles.Admin : Roles.User)
            }),
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes),
            SigningCredentials = credentials,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}
