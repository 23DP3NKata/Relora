using Relora.Admin.Application.Commands;
using Relora.Identity.Application.Interfaces;
using Relora.Persistance;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Relora.Admin.Application.Handlers.Commands;

public sealed class ChangeSupportRequestStatusCommandHandler : IRequestHandler<ChangeSupportRequestStatusCommand>
{
    private readonly ReloraDbContext _context;
    private readonly IUserRepository _userRepository;

    public ChangeSupportRequestStatusCommandHandler(ReloraDbContext context, IUserRepository userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }

    public async Task Handle(ChangeSupportRequestStatusCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(command.AdminId);

        if (user is null || !user.IsAdmin)
        {
            throw new UnauthorizedAccessException("User is not an admin.");
        }

        var supportRequest = await _context.SupportRequests
            .FirstOrDefaultAsync(x => x.Id == command.RequestId, cancellationToken);

        if (supportRequest is null)
        {
            throw new ArgumentException("Support request not found.");
        }

        supportRequest.ChangeStatus(command.Status);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
