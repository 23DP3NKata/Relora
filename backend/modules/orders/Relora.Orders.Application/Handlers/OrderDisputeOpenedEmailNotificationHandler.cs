using System.Net;

using Relora.Identity.Application.Interfaces;
using Relora.Orders.Domain.Events;
using Relora.Shared.Application.Emails;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Relora.Orders.Application.Handlers;

public sealed class OrderDisputeOpenedEmailNotificationHandler(
    IUserRepository userRepository,
    IEmailSender emailSender,
    ILogger<OrderDisputeOpenedEmailNotificationHandler> logger)
    : INotificationHandler<OrderDisputeOpenedDomainEvent>
{
    public async Task Handle(OrderDisputeOpenedDomainEvent notification, CancellationToken cancellationToken)
    {
        var admins = await userRepository.GetAdminUsersAsync(cancellationToken);

        if (admins.Count == 0)
        {
            logger.LogWarning("Order dispute {DisputeId} was opened, but no admin users were found.", notification.DisputeId);
            return;
        }

        var orderId = WebUtility.HtmlEncode(notification.OrderId.ToString());
        var disputeId = WebUtility.HtmlEncode(notification.DisputeId.ToString());
        var reason = WebUtility.HtmlEncode(notification.Reason);

        var body = $"""
            <p>A buyer opened a dispute.</p>
            <p><strong>Reason:</strong> {reason}</p>
            <p><strong>Order ID:</strong> {orderId}</p>
            <p><strong>Dispute ID:</strong> {disputeId}</p>
            <p>Review the order, lot, tracking number and evidence in the admin dispute queue.</p>
            """;

        foreach (var admin in admins)
        {
            try
            {
                await emailSender.SendAsync(
                    admin.Email,
                    "New Relora order dispute",
                    body,
                    cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "Dispute {DisputeId} admin email could not be sent to {AdminId}.",
                    notification.DisputeId,
                    admin.Id);
            }
        }
    }
}
