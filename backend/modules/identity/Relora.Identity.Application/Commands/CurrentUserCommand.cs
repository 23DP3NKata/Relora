using Relora.Identity.Application.Models;

using MediatR;

namespace Relora.Identity.Application.Commands;

public sealed record CurrentUserCommand(Guid userId) : IRequest<UserDto>;
