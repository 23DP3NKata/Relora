using System.Net;

using Relora.Admin.Application.Commands;
using Relora.Identity.Application.Interfaces;
using Relora.Items.Application.Interfaces;
using Relora.Shared.Application.Emails;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Relora.Admin.Application.Handlers.Commands;

public sealed class AcceptLotCommandHandler : IRequestHandler<AcceptLotCommand>
{
    private readonly ILotRepository _lotRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<AcceptLotCommandHandler> _logger;

    public AcceptLotCommandHandler(
        ILotRepository lotRepository,
        IUserRepository userRepository,
        IEmailSender emailSender,
        ILogger<AcceptLotCommandHandler> logger)
    {
        _lotRepository = lotRepository;
        _userRepository = userRepository;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task Handle(AcceptLotCommand command, CancellationToken cancellationToken)
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

        lot.Accept(command.adminId, DateTime.UtcNow);
        await _lotRepository.SaveLotAsync(lot, cancellationToken);
    }
}
