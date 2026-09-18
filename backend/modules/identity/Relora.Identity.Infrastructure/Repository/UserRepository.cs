
using Relora.Identity.Application.Interfaces;
using Relora.Identity.Application.Models;
using Relora.Identity.Domain;
using Relora.Persistance;

using Microsoft.EntityFrameworkCore;

namespace Relora.Identity.Infrastructure.Repository;
/// <summary>
/// Represents the user repository class.
/// </summary>
public class UserRepository(ReloraDbContext context) : IUserRepository
{
    private readonly ReloraDbContext _context = context;

    public async Task ChangePasswordAsync(User user, string passwordHash, CancellationToken cancellationToken)
    {
        user.ChangePassword(passwordHash);
        var tokens = await _context.RefreshTokens
            .Where(token => token.UserId == user.Id && token.RevokedAtUtc == null)
            .ToListAsync(cancellationToken);
        foreach (var token in tokens)
        {
            token.Revoke();
        }
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Adds user async.
    /// </summary>
    /// <param name="user">User.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        return _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates user async.
    /// </summary>
    /// <param name="user">User.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task UpdateUserAsync(User user)
    {
        _context.Entry(user).Property(value => value.Name).IsModified = true;
        _context.Entry(user).Property(value => value.UserName).IsModified = true;
        return _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes user async.
    /// </summary>
    /// <param name="user">User.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task DeleteUserAsync(User user)
    {
        _context.Users.Remove(user);
        return _context.SaveChangesAsync();
    }

    /// <summary>
    /// Gets user by email async.
    /// </summary>
    /// <param name="email">Email.</param>
    /// <returns>A task that returns the operation result.</returns>
    public Task<User?> GetUserByEmailAsync(string email)
    {
        return _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
    }

    public Task<User?> GetUserByUsernameAsync(string username)
    {
        return _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == username);
    }

    /// <summary>
    /// Gets user by id async.
    /// </summary>
    /// <param name="id">Entity identifier.</param>
    /// <returns>A task that returns the operation result.</returns>
    public Task<User?> GetUserByIdAsync(Guid id)
    {
        return _context.Users.FindAsync(id).AsTask();
    }

    public async Task<IReadOnlyList<User>> GetAdminUsersAsync(CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(user => user.IsAdmin)
            .ToListAsync(cancellationToken);
    }

    // queries with data
    public Task<int> GetUsersBidsPlacedCount(Guid id)
    {
        return _context.Bids.CountAsync(b => b.BidderId == id);
    }

    public Task<int> GetUserActiveLotsCountAsync(Guid id)
    {
        return _context.Lots.CountAsync(l => l.SellerId == id);
    }

    public Task<int> GetUserSoldLotsCountAsync(Guid id)
    {
        return _context.Lots.AsNoTracking().Where(l => l.SellerId == id && l.Status == Shared.Domain.Enums.LotStatus.Sold).CountAsync();
    }

    public Task<List<UserProfileListingDto>> GetUserActiveLotsAsync(Guid userId, int take = 4)
    {
        return _context.Lots.AsNoTracking()
            .Where(l => l.SellerId == userId)
            .Take(take)
            .Select(l => new UserProfileListingDto
            {
                LotId = l.Id,
                Title = l.Title,
                Brand = l.Brand,
                CurrentPrice = l.Price.Amount,
                Currency = l.Price.Currency,
                ThumbnailUrl = l.Media.FirstOrDefault() != null ? l.Media.FirstOrDefault().Key : null,
                Status = l.Status
            })
            .ToListAsync();
    }

    public Task<List<UserProfileListingDto>> GetUserSoldLotsAsync(Guid userId, int take = 4)
    {
        return _context.Lots.AsNoTracking()
            .Where(l => l.SellerId == userId && l.Status == Shared.Domain.Enums.LotStatus.Sold)
            .Take(take)
            .Select(l => new UserProfileListingDto
            {
                LotId = l.Id,
                Title = l.Title,
                Brand = l.Brand,
                CurrentPrice = l.Price.Amount,
                Currency = l.Price.Currency,
                ThumbnailUrl = l.Media.FirstOrDefault() != null ? l.Media.FirstOrDefault().Key : null,
                Status = l.Status
            })
            .ToListAsync();
    }
}
