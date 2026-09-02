using MediatR;

namespace Relora.Identity.Application.Commands;

public sealed record LogoutCommand(string? RefreshToken) : IRequest;
