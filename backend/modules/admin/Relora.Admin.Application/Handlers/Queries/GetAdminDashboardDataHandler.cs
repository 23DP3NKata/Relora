using Relora.Admin.Application.Queries;
using Relora.Admin.Domain.Models;
using Relora.Identity.Application.Interfaces;
using Relora.Persistance;
using Relora.Shared.Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Relora.Admin.Application.Handlers.Queries;

public sealed class GetAdminDashboardDataHandler : IRequestHandler<GetAdminDashboardData, AdminDashboardDto>
{
    private readonly ReloraDbContext _context;
    private readonly IUserRepository _userRepository;

    public GetAdminDashboardDataHandler(ReloraDbContext reloraDbContext, IUserRepository userRepository)
    {
        _context = reloraDbContext;
        _userRepository = userRepository;
    }

    public async Task<AdminDashboardDto> Handle(GetAdminDashboardData query, CancellationToken cancellationToken)
    {
        var user = _userRepository.GetUserByIdAsync(query.adminId);

        if (user == null)
        {
            throw new ArgumentException("User ot found");
        }

        if (!user.Result.IsAdmin)
        {
            throw new Exception("User not an admin");
        }

        var activeUsersCount = _context.Users.Count();
        var activeAuctionsCount = _context.Auctions.AsNoTracking().Where(a => a.Status == Shared.Domain.Enums.AuctionStatus.Active).Count();
        var pendingLotsCount = _context.Lots.AsNoTracking().Where(a => a.Status == LotStatus.Pending).Count();

        return new AdminDashboardDto
        {
            UsersCount = activeUsersCount,
            ActiveAuctionsCount = activeAuctionsCount,
            PendingLotsCount = pendingLotsCount
        };
    }
}
