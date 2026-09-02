using Relora.Admin.Application.Queries;
using Relora.Admin.Domain.Models;
using Relora.Identity.Application.Interfaces;
using Relora.Orders.Domain.Enums;
using Relora.Persistance;
using Relora.Shared.Domain.Time;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Relora.Admin.Application.Handlers.Queries;

public sealed class GetOverdueShipmentOrdersHandler
    : IRequestHandler<GetOverdueShipmentOrders, IReadOnlyList<OverdueShipmentOrderDto>>
{
    private readonly ReloraDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IClock _clock;

    public GetOverdueShipmentOrdersHandler(
        ReloraDbContext context,
        IUserRepository userRepository,
        IClock clock)
    {
        _context = context;
        _userRepository = userRepository;
        _clock = clock;
    }

    public async Task<IReadOnlyList<OverdueShipmentOrderDto>> Handle(
        GetOverdueShipmentOrders request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.AdminId);

        if (user is null || !user.IsAdmin)
        {
            throw new UnauthorizedAccessException("User is not an admin.");
        }

        var now = _clock.UtcNow;

        var overdueOrders = await _context.Orders
            .AsNoTracking()
            .Where(order =>
                order.Status == OrderStatus.AwaitingShipment &&
                order.ShipByUtc != null &&
                order.ShipByUtc < now)
            .OrderBy(order => order.ShipByUtc)
            .ToListAsync(cancellationToken);

        return overdueOrders
            .Select(order => new OverdueShipmentOrderDto
            {
                OrderId = order.Id,
                AuctionId = order.AuctionId,
                SellerId = order.SellerId,
                BuyerId = order.BuyerId,
                TotalPrice = order.TotalPrice,
                Currency = order.Currency,
                PaidAtUtc = order.PaidAtUtc,
                ShipByUtc = order.ShipByUtc,
                DaysOverdue = order.ShipByUtc == null
                    ? 0
                    : Math.Max(0, (int)Math.Floor((now - order.ShipByUtc.Value).TotalDays))
            })
            .ToList();
    }
}
