using Relora.Admin.Application.Queries;
using Relora.Admin.Domain.Models;
using Relora.Identity.Application.Interfaces;
using Relora.Items.Domain.Enums;
using Relora.Persistance;
using Relora.Shared.Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Relora.Admin.Application.Handlers.Queries;

public sealed class GetPendingLotsPreviewHandler
    : IRequestHandler<GetListOfPendingLots, IReadOnlyList<PendingLotPreviewDto>>
{
    private readonly ReloraDbContext _context;
    private readonly IUserRepository _userRepository;

    public GetPendingLotsPreviewHandler(ReloraDbContext context, IUserRepository userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<PendingLotPreviewDto>> Handle(GetListOfPendingLots request, CancellationToken cancellationToken)
    {
        var user = _userRepository.GetUserByIdAsync(request.adminId);

        if (user == null)
        {
            throw new ArgumentException("User ot found");
        }

        if (!user.Result.IsAdmin)
        {
            throw new Exception("User not an admin");
        }

        var pendingLots = await _context.Lots
            .AsNoTracking()
            .Where(l => l.Status == LotStatus.Pending)
            .Select(l => new PendingLotPreviewDto
            {
                Id = l.Id,
                Title = l.Title,
                Brand = l.Brand,
                PriceAmount = l.Price.Amount,
                Currency = l.Price.Currency,
                Condition = l.Condition,
                Status = l.Status,
                SellerId = l.SellerId,
                MainPhotoKey = l.Media
                    .Where(m => m.Type == "photo")
                    .Select(m => m.Key)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        return pendingLots;
    }
}
