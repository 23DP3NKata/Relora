using Relora.Items.Application.Commands;
using Relora.Items.Application.Interfaces;
using MediatR;

namespace Relora.Items.Application.Handlers.Commands;

public sealed class AcceptLotCommandHandler(ILotRepository lotRepository)
    : IRequestHandler<AcceptLotCommand, Guid>
{
    private readonly ILotRepository _lotRepository = lotRepository;

    public async Task<Guid> Handle(AcceptLotCommand request, CancellationToken cancellationToken)
    {
        var lot = await _lotRepository.GetLotById(request.LotId, cancellationToken)
            ?? throw new KeyNotFoundException($"Lot {request.LotId} not found.");

        lot.Publish();
        await _lotRepository.SaveLotAsync(lot, cancellationToken);

        return lot.Id;
    }
}
