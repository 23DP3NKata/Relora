using Relora.Identity.Application.Models;

using MediatR;

namespace Relora.Identity.Application.Commands;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResult>;
