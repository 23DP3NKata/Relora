namespace Relora.Identity.Domain;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public string? ReplacedByToken { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
    public bool IsRevoked => RevokedAtUtc.HasValue;
    public bool IsActive => !IsExpired && !IsRevoked;

    private RefreshToken() { }

    public RefreshToken(Guid userId, string token, DateTime expiresAtUtc)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Token = token;
        CreatedAtUtc = DateTime.UtcNow;
        ExpiresAtUtc = expiresAtUtc;
    }

    public void Revoke(string? replacedByToken = null)
    {
        RevokedAtUtc = DateTime.UtcNow;
        ReplacedByToken = replacedByToken;
    }

    public static RefreshToken Create(Guid userId, string token, DateTime expiresAtUtc)
    {
        return new RefreshToken
        (
            userId,
            token,
            expiresAtUtc
        );
    }
}
