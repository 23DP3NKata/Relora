using System.Net;

using Relora.Identity.Application.Interfaces;
using Relora.Orders.Domain.Events;
using Relora.Shared.Application.Emails;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Relora.Orders.Application.Handlers;

public sealed class OrderShippedEmailNotificationHandler(
    IUserRepository userRepository,
    IEmailSender emailSender,
    ILogger<OrderShippedEmailNotificationHandler> logger)
    : INotificationHandler<OrderShippedDomainEvent>
{
    public async Task Handle(OrderShippedDomainEvent notification, CancellationToken cancellationToken)
    {
        var buyer = await userRepository.GetUserByIdAsync(notification.BuyerId);
        if (buyer is null)
        {
            logger.LogWarning(
                "Order {OrderId} was shipped, but buyer {BuyerId} was not found for email notification.",
                notification.OrderId,
                notification.BuyerId);
            return;
        }

        try
        {
            var carrierName = WebUtility.HtmlEncode(notification.CarrierName);
            var trackingNumber = WebUtility.HtmlEncode(notification.TrackingNumber);
            var orderId = WebUtility.HtmlEncode(notification.OrderId.ToString());

            var body = $"""
                <p>Your order has been shipped.</p>
                <p><strong>Carrier:</strong> {carrierName}</p>
                <p><strong>Tracking number:</strong> {trackingNumber}</p>
                <p>You can follow the shipment from your order page.</p>
                <p style="color:#6b7280;font-size:12px;">Order ID: {orderId}</p>
                """;

            await emailSender.SendAsync(
                buyer.Email,
                "Your Relora order has been shipped",
                body,
                cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Order {OrderId} was shipped, but the buyer notification email could not be sent.",
                notification.OrderId);
        }
    }
}
