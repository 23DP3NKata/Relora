using Relora.Admin.Application.Queries;
using Relora.Admin.Domain.Models;
using Relora.Identity.Application.Interfaces;
using Relora.Orders.Domain.Enums;
using Relora.Persistance;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Relora.Admin.Application.Handlers.Queries;

public sealed class GetDeliveryIssueOrdersHandler
    : IRequestHandler<GetDeliveryIssueOrders, IReadOnlyList<DeliveryIssueOrderDto>>
{
    private readonly ReloraDbContext _context;
    private readonly IUserRepository _userRepository;

    public GetDeliveryIssueOrdersHandler(
        ReloraDbContext context,
        IUserRepository userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<DeliveryIssueOrderDto>> Handle(
        GetDeliveryIssueOrders request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.AdminId);

        if (user is null || !user.IsAdmin)
        {
            throw new UnauthorizedAccessException("User is not an admin.");
        }

        var orders = await _context.Orders
            .AsNoTracking()
            .Where(order => order.Status == OrderStatus.BuyerProtection)
            .OrderBy(order => order.DeliveryIssueReportedAtUtc)
            .ToListAsync(cancellationToken);

        return orders
            .Select(order => new DeliveryIssueOrderDto
            {
                OrderId = order.Id,
                AuctionId = order.AuctionId,
                SellerId = order.SellerId,
                BuyerId = order.BuyerId,
                TotalPrice = order.TotalPrice,
                Currency = order.Currency,
                ShippedAtUtc = order.ShippedAtUtc,
                DeliveryIssueReportedAtUtc = order.DeliveryIssueReportedAtUtc,
                DeliveryIssueReason = order.DeliveryIssueReason,
                CarrierName = order.CarrierName,
                TrackingNumber = order.TrackingNumber
            })
            .ToList();
    }
}
