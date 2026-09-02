using System.Security.Claims;

namespace Relora.Identity.Infrastructure.Claims;

public static class ClaimsExtension
{
    public static Guid GetUserId(this IEnumerable<Claim> claims)
    {
        var userIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            throw new InvalidOperationException("User ID claim not found.");
        }

        return Guid.Parse(userIdClaim.Value);
    }

    public static Guid? TryGetUserId(this IEnumerable<Claim> claims)
    {
        var userIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        return Guid.TryParse(userIdClaim?.Value, out var userId)
            ? userId
            : null;
    }
}
