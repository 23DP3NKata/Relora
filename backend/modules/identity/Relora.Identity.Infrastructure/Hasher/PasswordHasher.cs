using Relora.Identity.Application.Interfaces;

using BCrypt.Net;

namespace Relora.Identity.Infrastructure.Hasher;

/// <summary>
/// Represents the password hasher class.
/// </summary>
internal class PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// Performs the hash password operation.
    /// </summary>
    /// <param name="password">Password.</param>
    /// <returns>The operation result.</returns>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 16);
    }
    /// <summary>
    /// Performs the verify password operation.
    /// </summary>
    /// <param name="hashedPassword">Hashed password.</param>
    /// <param name="providedPassword">Provided password.</param>
    /// <returns>True if the operation succeeds; otherwise, false.</returns>
    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
    }
}
