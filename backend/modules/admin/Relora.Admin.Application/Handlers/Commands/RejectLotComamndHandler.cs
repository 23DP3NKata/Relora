using System.Net;

using Relora.Admin.Application.Commands;
using Relora.Identity.Application.Interfaces;
using Relora.Items.Application.Interfaces;
using Relora.Shared.Application.Emails;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Relora.Admin.Application.Handlers.Commands;

public sealed class RejectLotComamndHandler : IRequestHandler<RejectLotCommand>
{
    private readonly ILotRepository _lotRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<RejectLotComamndHandler> _logger;

    public RejectLotComamndHandler(
        ILotRepository lotRepository,
        IUserRepository userRepository,
        IEmailSender emailSender,
        ILogger<RejectLotComamndHandler> logger)
    {
        _lotRepository = lotRepository;
        _userRepository = userRepository;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task Handle(RejectLotCommand command, CancellationToken cancellationToken)
    {
        var lot = await _lotRepository.GetLotById(command.lotId, cancellationToken);

        if (lot == null)
        {
            throw new ArgumentException("Lot is not found.");
        }

        var user = await _userRepository.GetUserByIdAsync(command.adminId);

        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }

        if (!user.IsAdmin)
        {
            throw new UnauthorizedAccessException("User is not an admin.");
        }

        lot.Reject(command.adminId, command.reason ?? string.Empty, DateTime.UtcNow);
        await _lotRepository.SaveLotAsync(lot, cancellationToken);
    }
}
