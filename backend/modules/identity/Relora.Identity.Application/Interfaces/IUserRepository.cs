using Relora.Identity.Application.Models;
using Relora.Identity.Domain;

namespace Relora.Identity.Application.Interfaces;

/// <summary>
/// Represents the user repository interface.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByIdAsync(Guid id);
    Task<IReadOnlyList<User>> GetAdminUsersAsync(CancellationToken cancellationToken);
    Task<User?> GetUserByUsernameAsync(string username);

    Task AddUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task ChangePasswordAsync(User user, string passwordHash, CancellationToken cancellationToken);
    Task DeleteUserAsync(User user);

    Task<int> GetUsersBidsPlacedCount(Guid userId);
    Task<int> GetUserActiveLotsCountAsync(Guid userId);
    Task<int> GetUserSoldLotsCountAsync(Guid userId);

    Task<List<UserProfileListingDto>> GetUserActiveLotsAsync(Guid userId, int take = 4);
    Task<List<UserProfileListingDto>> GetUserSoldLotsAsync(Guid userId, int take = 4);
}
