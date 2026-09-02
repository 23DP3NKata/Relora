using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Relora.Identity.Application.Commands;
using Relora.Identity.Application.Interfaces;
using Relora.Identity.Application.Models;
using Relora.Identity.Domain;
using Relora.Shared.Domain.Exceptions;

using MediatR;
using Microsoft.AspNetCore.Http;

namespace Relora.Identity.Application.Handlers;
/// <summary>
/// Represents the register command handler class.
/// </summary>
public sealed class RegisterCommandHandler(
    ITokenProvider tokenProvider,
    IPasswordHasher passwordHasher,
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository) : IRequestHandler<RegisterCommand, AuthResult>
{
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

    /// <summary>
    /// Handles the operation.
    /// </summary>
    /// <param name="request">Input data for the operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.email))
        {
            throw new ArgumentException("Email is required");
        }

        if (string.IsNullOrEmpty(request.username))
        {
            throw new ArgumentException("username is required");
        }

        if (string.IsNullOrEmpty(request.password))
        {
            throw new ArgumentException("password is required");
        }

        if (string.IsNullOrEmpty(request.confirmPassword))
        {
            throw new ArgumentException("password is required");
        }

        if (request.password != request.confirmPassword)
        {
            throw new ArgumentException("Passwords do not match");
        }

        var existingUserByEmail = await _userRepository.GetUserByEmailAsync(request.email);
        if (existingUserByEmail is not null)
        {
            throw new Error("A user with this email already exists.", StatusCodes.Status409Conflict);
        }

        var existingUserByUsername = await _userRepository.GetUserByUsernameAsync(request.username);
        if (existingUserByUsername is not null)
        {
            throw new Error("This username is already taken.", StatusCodes.Status409Conflict);
        }

        var hashedPassword = _passwordHasher.HashPassword(request.password);

        var user = User.Create(request.username, request.email, hashedPassword, request.username);

        await _userRepository.AddUserAsync(user);

        var accessToken =  _tokenProvider.GenerateAccessToken(user);
        var refreshTokenValue =  _tokenProvider.GenerateRefreshToken();

        var refreshToken = RefreshToken.Create(user.Id, refreshTokenValue, DateTime.UtcNow.AddDays(7));

        await _refreshTokenRepository.AddRefreshToken(refreshToken);

        return new AuthResult(accessToken, refreshTokenValue);
    }
}
