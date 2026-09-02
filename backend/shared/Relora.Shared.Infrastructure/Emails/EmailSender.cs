using System.Net;
using System.Net.Mail;

using Relora.Shared.Application.Emails;

using Microsoft.Extensions.Options;

namespace Relora.Shared.Infrastructure.Emails;

public sealed class EmailSender(IOptions<EmailOptions> options) : IEmailSender
{
    private readonly EmailOptions _options = options.Value;

    public async Task SendAsync(string recipientEmail, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(_options.FromEmail, _options.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(recipientEmail);

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            Credentials = new NetworkCredential(_options.Username, _options.Password)
        };

        await client.SendMailAsync(message, cancellationToken);
    }
}
