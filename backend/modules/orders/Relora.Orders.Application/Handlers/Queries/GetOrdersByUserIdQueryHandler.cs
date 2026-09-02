using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

using Relora.Orders.Application.Queries;
using Relora.Orders.Application.Interfaces;
using Relora.Orders.Domain;
using Relora.Orders.Application.Models;

namespace Relora.Orders.Application.Handlers.Queries;

/// <summary>
/// Represents the get orders by user id query handler class.
/// </summary>
public sealed class GetOrdersByUserIdQueryHandler : IRequestHandler<GetOrdersByUserIdQuery, IReadOnlyList<OrderListItemDto>>
{
    private readonly IOrderRepository _orderRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetOrdersByUserIdQueryHandler"/> class.
    /// </summary>
    /// <param name="orderRepository">Order repository.</param>
    public GetOrdersByUserIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    /// <summary>
    /// Handles the operation.
    /// </summary>
    /// <param name="request">Input data for the operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<IReadOnlyList<OrderListItemDto>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
    {
        var ordersAsBuyerTask = await _orderRepository.GetOrdersByBuyerId(request.userId, cancellationToken);

        var ordersAsSellerTask = await _orderRepository.GetOrdersBySellerId(request.userId, cancellationToken);


        var allOrders = ordersAsBuyerTask
                   .Concat(ordersAsSellerTask)
                   .GroupBy(o => o.Id)
                   .Select(g => g.First())
                   .OrderByDescending(o => o.PaidAtUtc)
                   .ToList();

        var dto = allOrders.Select(o => new OrderListItemDto
        {
            Id = o.Id,
            AuctionId = o.AuctionId,
            SellerId = o.SellerId,
            BuyerId = o.BuyerId,
            Status = o.Status,
            OrderStatusName = o.Status.ToString(),
            Price = o.Price,
            ShippingPrice = o.ShippingPrice,
            TotalPrice = o.TotalPrice,
            Currency = o.Currency,
            ShippingCurrency = o.ShippingCurrency,
            ShippingHandlingDays = o.ShippingHandlingDays,
            ShipByUtc = o.ShipByUtc,
            ShippedAtUtc = o.ShippedAtUtc,
            DeliveredAtUtc = o.DeliveredAtUtc,
            CompletedAtUtc = o.CompletedAtUtc,
            DeliveryIssueAvailableFromUtc = o.DeliveryIssueAvailableFromUtc,
            DeliveryIssueReportedAtUtc = o.DeliveryIssueReportedAtUtc,
            DeliveryIssueReason = o.DeliveryIssueReason,
            CarrierName = o.CarrierName,
            TrackingNumber = o.TrackingNumber,
            PaymentDeadlineUtc = o.PaymentDeadlineUtc,
            PaidAtUtc = o.PaidAtUtc
        }).ToList();

        return dto;
    }
}
