using Relora.Identity.Application.Queries;
using Relora.Identity.Application.Models;
using MediatR;

using Relora.Identity.Application.Interfaces;
using Relora.Persistance;

namespace Relora.Identity.Application.Handlers.Queries;

public sealed class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ReloraDbContext _reloraDbContext;

    public GetUserProfileQueryHandler(IUserRepository userRepository, ReloraDbContext reloraDbContext)
    {
        _userRepository = userRepository;
        _reloraDbContext = reloraDbContext;
    }

    public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUsernameAsync(request.username);

        if (user is null)
        {
            throw new KeyNotFoundException($"User with ID {request.username} not found.");
        }

        return new UserProfileDto
        {
            Id = user.Id,
            Username = user.UserName,
            Name = user.Name,
            Stats = new UserProfileStatsDto
            {
                BidsPlaced = await _userRepository.GetUsersBidsPlacedCount(user.Id),
                ActiveListingsCount = await _userRepository.GetUserActiveLotsCountAsync(user.Id),
                SoldItemsCount = await _userRepository.GetUserSoldLotsCountAsync(user.Id)
            },
            ActiveListings = await _userRepository.GetUserActiveLotsAsync(user.Id),
            SoldListings = await _userRepository.GetUserSoldLotsAsync(user.Id)
        };
    }
}
