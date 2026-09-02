using Relora.Identity.Application.Commands;
using Relora.Identity.Application.Interfaces;
using Relora.Identity.Application.Models;

using MediatR;

namespace Relora.Identity.Application.Handlers;
public sealed class CurrentUserCommandHandler : IRequestHandler<CurrentUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;

    public CurrentUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<UserDto> Handle(CurrentUserCommand command, CancellationToken ct)
    {
        var user = _userRepository.GetUserByIdAsync(command.userId);

        if (user is null)
        {
            throw new UnauthorizedAccessException("No user is currently authenticated.");
        }

        var userDto = new UserDto(user.Result.Id, user.Result.UserName, user.Result.Email);

        return Task.FromResult(userDto);
    }
}
