using System.Net;

using Relora.Identity.Application.Interfaces;
using Relora.Orders.Domain.Events;
using Relora.Shared.Application.Emails;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Relora.Orders.Application.Handlers;

public sealed class OrderDisputeResolvedEmailNotificationHandler(
    IUserRepository userRepository,
    IEmailSender emailSender,
    ILogger<OrderDisputeResolvedEmailNotificationHandler> logger)
    : INotificationHandler<OrderDisputeResolvedDomainEvent>
{
    public async Task Handle(OrderDisputeResolvedDomainEvent notification, CancellationToken cancellationToken)
    {
        var buyer = await userRepository.GetUserByIdAsync(notification.BuyerId);
        var seller = await userRepository.GetUserByIdAsync(notification.SellerId);

        var decision = WebUtility.HtmlEncode(notification.Decision);
        var decisionReason = WebUtility.HtmlEncode(notification.DecisionReason);
        var orderId = WebUtility.HtmlEncode(notification.OrderId.ToString());

        var body = $"""
            <p>The dispute for order {orderId} has been resolved.</p>
            <p><strong>Decision:</strong> {decision}</p>
            <p><strong>Reason:</strong> {decisionReason}</p>
            """;

        await SendIfPossible(buyer?.Email, "Your Relora dispute has been resolved", body, notification.DisputeId, cancellationToken);
        await SendIfPossible(seller?.Email, "An Relora order dispute has been resolved", body, notification.DisputeId, cancellationToken);
    }

    private async Task SendIfPossible(
        string? email,
        string subject,
        string body,
        Guid disputeId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            logger.LogWarning("Dispute {DisputeId} result email was skipped because recipient email is missing.", disputeId);
            return;
        }

        try
        {
            await emailSender.SendAsync(email, subject, body, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Dispute {DisputeId} result email could not be sent.", disputeId);
        }
    }
}
