using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using Relora.Items.Application.Commands;
using Relora.Items.Application.Interfaces;

namespace Relora.Items.Application.Handlers.Commands;
/// <summary>
/// Represents the publish lot command handler class.
/// </summary>
public sealed class PublishLotCommandHandler : IRequestHandler<PublishLotCommand, Guid>
{
    private readonly ILotRepository _lotRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublishLotCommandHandler"/> class.
    /// </summary>
    /// <param name="lotRepository">Lot repository.</param>
    public PublishLotCommandHandler(ILotRepository lotRepository)
    {
        _lotRepository = lotRepository;
    }

    /// <summary>
    /// Handles the operation.
    /// </summary>
    /// <param name="request">Input data for the operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<Guid> Handle(PublishLotCommand request, CancellationToken cancellationToken)
    {
        var lot = await _lotRepository.GetLotById(request.LotId, cancellationToken);

        if (lot == null)
        {
            throw new InvalidOperationException("Lot not found.");
        }

        if (lot.SellerId != request.SellerId)
        {
            throw new UnauthorizedAccessException("Seller does not own the lot.");
        }

        lot.Submit();

        await _lotRepository.SaveLotAsync(lot, cancellationToken);
        return lot.Id;
    }
}
