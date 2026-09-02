using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Relora.Admin.Application.Commands;
using Relora.Identity.Application.Interfaces;
using Relora.Items.Application.Interfaces;

using MediatR;

namespace Relora.Admin.Application.Handlers.Commands;
public sealed class DeleteLotCommandHandler : IRequestHandler<DeleteLotCommand>
{
    private readonly ILotRepository _lotRepository;
    private readonly IUserRepository _userRepository;

    public DeleteLotCommandHandler(ILotRepository lotRepository, IUserRepository userRepository)
    {
        _lotRepository = lotRepository;
        _userRepository = userRepository;
    }

    public async Task Handle(DeleteLotCommand command, CancellationToken cancellationToken)
    {
        var lot = await _lotRepository.GetLotById(command.lotId, cancellationToken);

        if (lot == null)
        {
            throw new ArgumentException("Lot is not found");
        }

        var user = _userRepository.GetUserByIdAsync(command.adminId);

        if (user == null)
        {
            throw new ArgumentException("User ot found");
        }

        if (!user.Result.IsAdmin)
        {
            throw new Exception("User not an admin");
        }

        await _lotRepository.DeleteLotAsync(lot, cancellationToken);
    }
}
