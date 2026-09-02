namespace Relora.Identity.Application.Interfaces;

/// <summary>
/// Represents the i password hasher interface.
/// </summary>
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string providedPassword);
}
