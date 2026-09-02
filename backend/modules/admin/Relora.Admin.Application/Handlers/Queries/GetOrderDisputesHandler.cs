using Relora.Admin.Application.Queries;
using Relora.Admin.Domain.Models;
using Relora.Identity.Application.Interfaces;
using Relora.Orders.Domain.Enums;
using Relora.Persistance;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Relora.Admin.Application.Handlers.Queries;

public sealed class GetOrderDisputesHandler
    : IRequestHandler<GetOrderDisputes, IReadOnlyList<AdminOrderDisputeDto>>
{
    private readonly ReloraDbContext _context;
    private readonly IUserRepository _userRepository;

    public GetOrderDisputesHandler(
        ReloraDbContext context,
        IUserRepository userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<AdminOrderDisputeDto>> Handle(
        GetOrderDisputes request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.AdminId);

        if (user is null || !user.IsAdmin)
        {
            throw new UnauthorizedAccessException("User is not an admin.");
        }

        var disputes = await _context.OrderDisputes
            .AsNoTracking()
            .Include(dispute => dispute.Evidence)
            .OrderBy(dispute => dispute.Status == OrderDisputeStatus.Open ? 0 : 1)
            .ThenByDescending(dispute => dispute.OpenedAtUtc)
            .ToListAsync(cancellationToken);

        var orderIds = disputes
            .Select(dispute => dispute.OrderId)
            .Distinct()
            .ToList();

        var orders = await _context.Orders
            .AsNoTracking()
            .Where(order => orderIds.Contains(order.Id))
            .ToDictionaryAsync(order => order.Id, cancellationToken);

        var auctionIds = orders.Values
            .Select(order => order.AuctionId)
            .Distinct()
            .ToList();

        var auctions = await _context.Auctions
            .AsNoTracking()
            .Where(auction => auctionIds.Contains(auction.Id))
            .ToDictionaryAsync(auction => auction.Id, cancellationToken);

        var lotIds = auctions.Values
            .Where(auction => auction.LotId.HasValue)
            .Select(auction => auction.LotId!.Value)
            .Distinct()
            .ToList();

        var lots = await _context.Lots
            .AsNoTracking()
            .Include(lot => lot.Media)
            .Where(lot => lotIds.Contains(lot.Id))
            .ToDictionaryAsync(lot => lot.Id, cancellationToken);

        return disputes
            .Where(dispute => orders.ContainsKey(dispute.OrderId))
            .Select(dispute =>
            {
                var order = orders[dispute.OrderId];
                auctions.TryGetValue(order.AuctionId, out var auction);
                var lotId = auction?.LotId;
                var lot = lotId.HasValue && lots.TryGetValue(lotId.Value, out var foundLot)
                    ? foundLot
                    : null;

                return new AdminOrderDisputeDto
                {
                    DisputeId = dispute.Id,
                    OrderId = order.Id,
                    AuctionId = order.AuctionId,
                    LotId = lotId,
                    LotTitle = lot?.Title,
                    LotBrand = lot?.Brand,
                    LotPhotoKey = lot?.Media.FirstOrDefault(media => media.Type == "photo")?.Key,
                    SellerId = order.SellerId,
                    BuyerId = order.BuyerId,
                    TotalPrice = order.TotalPrice,
                    Currency = order.Currency,
                    OrderStatusName = order.Status.ToString(),
                    ReasonName = dispute.Reason.ToString(),
                    Description = dispute.Description,
                    DisputeStatusName = dispute.Status.ToString(),
                    OpenedAtUtc = dispute.OpenedAtUtc,
                    ResolvedAtUtc = dispute.ResolvedAtUtc,
                    ResolvedByAdminId = dispute.ResolvedByAdminId,
                    DecisionName = dispute.Decision?.ToString(),
                    DecisionReason = dispute.DecisionReason,
                    PayoutBlocked = dispute.Status == OrderDisputeStatus.Open && order.Status == OrderStatus.Disputed,
                    ShippedAtUtc = order.ShippedAtUtc,
                    CarrierName = order.CarrierName,
                    TrackingNumber = order.TrackingNumber,
                    Evidence = dispute.Evidence
                        .OrderBy(evidence => evidence.CreatedAtUtc)
                        .Select(evidence => new AdminOrderDisputeEvidenceDto
                        {
                            Id = evidence.Id,
                            Key = evidence.Key,
                            CreatedAtUtc = evidence.CreatedAtUtc
                        })
                        .ToList()
                };
            })
            .ToList();
    }
}
