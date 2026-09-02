namespace Relora.Identity.Infrastructure.Configuration;

/// <summary>
/// Represents the jwt options class.
/// </summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    /// <summary>
    /// Gets or sets the secret used by this type.
    /// </summary>
    public string Secret { get; init; } = string.Empty;
    /// <summary>
    /// Gets or sets the issuer used by this type.
    /// </summary>
    public string Issuer { get; init; } = string.Empty;
    /// <summary>
    /// Gets or sets the audience used by this type.
    /// </summary>
    public string Audience { get; init; } = string.Empty;
    /// <summary>
    /// Gets or sets the expiration in minutes used by this type.
    /// </summary>
    public int ExpirationInMinutes { get; init; }
}

