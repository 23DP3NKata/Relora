namespace Relora.Shared.Application.Emails;

public interface IEmailSender
{
    Task SendAsync(string recipientEmail, string subject, string htmlBody, CancellationToken cancellationToken = default);
}
