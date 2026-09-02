using System.Net;

using Relora.Identity.Application.Interfaces;
using Relora.Orders.Domain.Events;
using Relora.Shared.Application.Emails;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Relora.Orders.Application.Handlers;

public sealed class OrderDeliveryIssueEmailNotificationHandler(
    IUserRepository userRepository,
    IEmailSender emailSender,
    ILogger<OrderDeliveryIssueEmailNotificationHandler> logger)
    : INotificationHandler<OrderDeliveryIssueReportedDomainEvent>
{
    public async Task Handle(OrderDeliveryIssueReportedDomainEvent notification, CancellationToken cancellationToken)
    {
        var seller = await userRepository.GetUserByIdAsync(notification.SellerId);
        if (seller is null)
        {
            logger.LogWarning(
                "Order {OrderId} delivery issue was reported, but seller {SellerId} was not found.",
                notification.OrderId,
                notification.SellerId);
            return;
        }

        try
        {
            var orderId = WebUtility.HtmlEncode(notification.OrderId.ToString());
            var reason = WebUtility.HtmlEncode(notification.Reason ?? "No additional details provided.");

            var body = $"""
                <p>The buyer reported that the shipped item has not been delivered.</p>
                <p><strong>Buyer message:</strong> {reason}</p>
                <p>Please check the carrier tracking and be ready to help the Relora team resolve the case.</p>
                <p style="color:#6b7280;font-size:12px;">Order ID: {orderId}</p>
                """;

            await emailSender.SendAsync(
                seller.Email,
                "Delivery issue reported for your Relora order",
                body,
                cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Order {OrderId} delivery issue was saved, but seller email notification could not be sent.",
                notification.OrderId);
        }
    }
}
