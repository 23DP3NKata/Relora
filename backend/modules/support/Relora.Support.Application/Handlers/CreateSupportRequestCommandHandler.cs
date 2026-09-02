using Relora.Shared.Application.Emails;
using Relora.Shared.Application.Emails.Templates;
using Relora.Support.Application.Commands;
using Relora.Support.Application.Interfaces;
using Relora.Support.Domain;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Relora.Support.Application.Handlers;

public sealed class CreateSupportRequestCommandHandler(
    ISupportRequestRepository repository,
    IEmailSender emailSender,
    ILogger<CreateSupportRequestCommandHandler> logger) : IRequestHandler<CreateSupportRequestCommand, Guid>
{
    public async Task<Guid> Handle(CreateSupportRequestCommand request, CancellationToken cancellationToken)
    {
        var supportRequest = new SupportRequest(Guid.NewGuid(), request.Email, request.Category, request.Subject, request.Message, DateTime.UtcNow);
        await repository.AddAsync(supportRequest, cancellationToken);

        try
        {
            var body = SupportRequestReceivedEmailTemplate.Create(request.Subject, supportRequest.Id.ToString());

            await emailSender.SendAsync(
                request.Email,
                "We received your support request",
                body,
                cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Support request {SupportRequestId} was saved, but the confirmation email could not be sent.", supportRequest.Id);
        }

        return supportRequest.Id;
    }
}