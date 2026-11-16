using Relora.Admin.Application.Queries;
using Relora.Admin.Domain.Models;
using Relora.Identity.Application.Interfaces;
using Relora.Persistance;
using Relora.Support.Domain;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Relora.Admin.Application.Handlers.Queries;

public sealed class GetSupportRequestsHandler
    : IRequestHandler<GetSupportRequests, IReadOnlyList<AdminSupportRequestDto>>
{
    private readonly ReloraDbContext _context;
    private readonly IUserRepository _userRepository;

    public GetSupportRequestsHandler(ReloraDbContext context, IUserRepository userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<AdminSupportRequestDto>> Handle(
        GetSupportRequests request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.AdminId);

        if (user is null || !user.IsAdmin)
        {
            throw new UnauthorizedAccessException("User is not an admin.");
        }

        var supportRequests = await _context.SupportRequests
            .AsNoTracking()
            .OrderBy(x => x.Status == SupportRequestStatus.New ? 0 : x.Status == SupportRequestStatus.InProgress ? 1 : 2)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        // lietotājvārdi tiem, kas sūtīja ielogojušies
        var userIds = supportRequests
            .Where(x => x.UserId.HasValue)
            .Select(x => x.UserId!.Value)
            .Distinct()
            .ToList();

        var usernames = await _context.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.UserName, cancellationToken);

        return supportRequests
            .Select(x => new AdminSupportRequestDto
            {
                Id = x.Id,
                UserId = x.UserId,
                Username = x.UserId.HasValue && usernames.TryGetValue(x.UserId.Value, out var username) ? username : null,
                Email = x.Email,
                Category = x.Category,
                Subject = x.Subject,
                Message = x.Message,
                StatusName = x.Status.ToString(),
                CreatedAt = x.CreatedAt
            })
            .ToList();
    }
}
