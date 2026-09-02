using Relora.Identity.Application.Commands;
using Relora.Identity.Application.Interfaces;
using Relora.Identity.Application.Models;
using Relora.Identity.Domain;

using MediatR;

namespace Relora.Identity.Application.Handlers;

public sealed class RefreshCommandHandler(
    ITokenProvider tokenProvider,
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository)
    : IRequestHandler<RefreshTokenCommand, AuthResult>
{
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new UnauthorizedAccessException("Refresh token is missing.");
        }

        var existingRefreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

        if (existingRefreshToken is null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        if (!existingRefreshToken.IsActive)
        {
            throw new UnauthorizedAccessException("Refresh token is no longer active.");
        }

        var user = await _userRepository.GetUserByIdAsync(existingRefreshToken.UserId);

        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        var newAccessToken = _tokenProvider.GenerateAccessToken(user);
        var newRefreshTokenValue = _tokenProvider.GenerateRefreshToken();

        existingRefreshToken.Revoke(newRefreshTokenValue);

        var newRefreshToken = RefreshToken.Create(user.Id, newRefreshTokenValue, DateTime.UtcNow.AddDays(7));

        await _refreshTokenRepository.AddRefreshToken(newRefreshToken);

        return new AuthResult(newAccessToken, newRefreshTokenValue);
    }
}
