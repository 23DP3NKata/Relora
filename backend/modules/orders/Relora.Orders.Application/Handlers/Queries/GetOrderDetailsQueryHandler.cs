using Relora.Orders.Application.Models;
using Relora.Orders.Application.Queries;
using Relora.Orders.Application.Interfaces;

using System.Linq;

using MediatR;

namespace Relora.Orders.Application.Handlers.Queries;
/// <summary>
/// Represents the get order details query handler class.
/// </summary>
public sealed class GetOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailsQuery, OrderDetailsDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderDisputeRepository _disputeRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetOrderDetailsQueryHandler"/> class.
    /// </summary>
    /// <param name="orderRepository">Order repository.</param>
    public GetOrderDetailsQueryHandler(
        IOrderRepository orderRepository,
        IOrderDisputeRepository disputeRepository)
    {
        _orderRepository = orderRepository;
        _disputeRepository = disputeRepository;
    }
    /// <summary>
    /// Handles the operation.
    /// </summary>
    /// <param name="request">Input data for the operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<OrderDetailsDto> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderByIdAsync(request.orderId, cancellationToken);

        if (order is null)
        {
            throw new KeyNotFoundException($"Order {request.orderId} not found.");
        }

        if (order.BuyerId != request.userId && order.SellerId != request.userId)
        {
            throw new UnauthorizedAccessException("This order does not belong to the user.");
        }

        var canViewShippingAddress =
            order.BuyerId == request.userId ||
            (order.SellerId == request.userId && order.PaidAtUtc != null);
        var dispute = await _disputeRepository.GetLatestByOrderIdAsync(order.Id, cancellationToken);

        var dto = new OrderDetailsDto
        {
            Id = order.Id,
            AuctionId = order.AuctionId,
            SellerId = order.SellerId,
            BuyerId = order.BuyerId,
            Status = order.Status,
            OrderStatusName = order.Status.ToString(),
            Price = order.Price,
            ShippingPrice = order.ShippingPrice,
            TotalPrice = order.TotalPrice,
            Currency = order.Currency,
            ShippingCurrency = order.ShippingCurrency,
            ShippingOriginCountry = order.ShippingOriginCountry,
            ShipsToCountries = order.ShipsToCountries,
            ShippingHandlingDays = order.ShippingHandlingDays,
            ShipByUtc = order.ShipByUtc,
            ShippedAtUtc = order.ShippedAtUtc,
            DeliveredAtUtc = order.DeliveredAtUtc,
            CompletedAtUtc = order.CompletedAtUtc,
            DeliveryIssueAvailableFromUtc = order.DeliveryIssueAvailableFromUtc,
            DeliveryIssueReportedAtUtc = order.DeliveryIssueReportedAtUtc,
            DeliveryIssueReason = order.DeliveryIssueReason,
            CarrierName = order.CarrierName,
            TrackingNumber = order.TrackingNumber,
            CanViewShippingAddress = canViewShippingAddress,
            ShippingAddress = canViewShippingAddress
                ? new ShippingAddressDto
                {
                    FullName = order.ShippingFullName,
                    CountryCode = order.ShippingCountryCode,
                    Country = order.ShippingCountry,
                    City = order.ShippingCity,
                    PostalCode = order.ShippingPostalCode,
                    AddressLine1 = order.ShippingAddressLine1,
                    AddressLine2 = order.ShippingAddressLine2,
                    Phone = order.ShippingPhone
                }
                : null,
            PaymentDeadlineUtc = order.PaymentDeadlineUtc,
            StripeCheckoutSessionId = order.StripeCheckoutSessionId,
            PaidAtUtc = order.PaidAtUtc,
            Dispute = dispute is null
                ? null
                : new OrderDisputeDto
                {
                    Id = dispute.Id,
                    OrderId = dispute.OrderId,
                    Reason = dispute.Reason,
                    ReasonName = dispute.Reason.ToString(),
                    Description = dispute.Description,
                    Status = dispute.Status,
                    StatusName = dispute.Status.ToString(),
                    OpenedAtUtc = dispute.OpenedAtUtc,
                    ResolvedAtUtc = dispute.ResolvedAtUtc,
                    ResolvedByAdminId = dispute.ResolvedByAdminId,
                    Decision = dispute.Decision,
                    DecisionName = dispute.Decision?.ToString(),
                    DecisionReason = dispute.DecisionReason,
                    Evidence = dispute.Evidence
                        .Select(evidence => new OrderDisputeEvidenceDto
                        {
                            Id = evidence.Id,
                            Key = evidence.Key,
                            CreatedAtUtc = evidence.CreatedAtUtc
                        })
                        .ToList()
                }
        };

        return dto;
    }
}
