using Relora.Identity.Application.Commands;
using Relora.Identity.Application.Interfaces;

using MediatR;

namespace Relora.Identity.Application.Handlers;

public sealed class LogoutCommandHandler(
    IRefreshTokenRepository refreshTokenRepository) : IRequestHandler<LogoutCommand>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return;
        }

        var existingRefreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

        if (existingRefreshToken is null)
        {
            return;
        }

        if (!existingRefreshToken.IsActive)
        {
            return;
        }

        existingRefreshToken.Revoke();

        await _refreshTokenRepository.SaveRefreshTokenAsync(existingRefreshToken);
    }
}
