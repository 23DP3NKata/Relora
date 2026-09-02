using Relora.Identity.Application.Commands;
using Relora.Identity.Application.Interfaces;
using Relora.Identity.Application.Models;
using Relora.Identity.Domain;

using MediatR;

using Microsoft.IdentityModel.Tokens.Experimental;

namespace Relora.Identity.Application.Handlers;

/// <summary>
/// Represents the login command handler class.
/// </summary>
public class LoginCommandHandler(
    ITokenProvider tokenProvider,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IRefreshTokenRepository refreshTokenRepository) : IRequestHandler<LoginCommand, AuthResult>
{
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

    /// <summary>
    /// Handles the operation.
    /// </summary>
    /// <param name="request">Input data for the operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.email);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.password))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var accessToken = _tokenProvider.GenerateAccessToken(user);
        var refreshTokenValue = _tokenProvider.GenerateRefreshToken();

        var refreshToken = RefreshToken.Create(user.Id, refreshTokenValue, DateTime.UtcNow.AddDays(7));

        await _refreshTokenRepository.AddRefreshToken(refreshToken);

        return new AuthResult(accessToken, refreshTokenValue);
    }
}
