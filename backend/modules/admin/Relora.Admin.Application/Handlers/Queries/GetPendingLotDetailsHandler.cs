using Relora.Admin.Application.Queries;
using Relora.Admin.Domain.Models;
using Relora.Identity.Application.Interfaces;
using Relora.Identity.Infrastructure.Repository;
using Relora.Persistance;
using Relora.Shared.Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Relora.Admin.Application.Handlers.Queries;

public sealed class GetPendingLotDetailsHandler
    : IRequestHandler<GetPendingLotDetails, PendingLotPreviewDetailsDto?>
{
    private readonly ReloraDbContext _context;
    private readonly IUserRepository UserRepository;

    public GetPendingLotDetailsHandler(ReloraDbContext context, IUserRepository userRepository)
    {
        _context = context;
        UserRepository = userRepository;
    }

    public async Task<PendingLotPreviewDetailsDto?> Handle(
        GetPendingLotDetails query,
        CancellationToken cancellationToken)
    {

        var user = UserRepository.GetUserByIdAsync(query.adminId);

        if (user == null)
        {
            throw new ArgumentException("User ot found");
        }

        if (!user.Result.IsAdmin)
        {
            throw new Exception("User not an admin");
        }

        return await _context.Lots
            .AsNoTracking()
            .Where(l => l.Id == query.LotId && l.Status == LotStatus.Pending)
            .Select(l => new PendingLotPreviewDetailsDto
            {
                Id = l.Id,
                Title = l.Title,
                DescriptionPreview = l.Description,
                SellerId = l.SellerId,

                SellerUsername = _context.Users
                    .Where(u => u.Id == l.SellerId)
                    .Select(u => u.UserName)
                    .FirstOrDefault(),

                SellerEmail = _context.Users
                    .Where(u => u.Id == l.SellerId)
                    .Select(u => u.Email)
                    .FirstOrDefault(),

                Brand = l.Brand,
                Category = l.Category,
                Condition = l.Condition,
                StartingPrice = l.Price.Amount,
                Currency = l.Price.Currency,
                Status = l.Status,

                MainImageUrl = l.Media
                    .Where(m => m.Type == "photo")
                    .Select(m => m.Key)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
