using Relora.Auctions.Application.Commands;
using Relora.Auctions.Application.Interfaces;
using Relora.Shared.Domain.Time;
using Relora.Shared.Domain.Persistence;
using Relora.Shared.Infrastructure.Interfaces;
using Relora.Shared.Domain.Abstractions;

using MediatR;
using Relora.Items.Application.Interfaces;
using Relora.Items.Domain;

namespace Relora.Auctions.Application.Handlers.Commands;
/// <summary>
/// Represents the stop auction command handler class.
/// </summary>
public sealed class StopAuctionCommandHandler : IRequestHandler<StopAuctionCommand>
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly ILotRepository _lotRepository;
    private readonly IDomainEventDispatcher _domainEventHandler;
    private readonly IClock _clock;
    private readonly ITransactionRunner _transactionRunner;

    public StopAuctionCommandHandler(
        IAuctionRepository auctionRepository,
        IClock clock,
        IDomainEventDispatcher domainEventHandler,
        ILotRepository lotRepository,
        ITransactionRunner transactionRunner)
    {
        _auctionRepository = auctionRepository;
        _clock = clock;
        _domainEventHandler = domainEventHandler;
        _lotRepository = lotRepository;
        _transactionRunner = transactionRunner;
    }

    /// <summary>
    /// Handles the operation.
    /// </summary>
    /// <param name="request">Input data for the operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Handle(StopAuctionCommand request, CancellationToken cancellationToken)
    {
        var auction = await _auctionRepository.GetAuctionById(request.AuctionId, cancellationToken);

        if (auction == null)
        {
            throw new InvalidOperationException("Auction not found.");
        }

        var lot = await _lotRepository.GetLotById(auction.LotId, cancellationToken);

        if (lot == null)
        {
            throw new InvalidOperationException("Lot not found.");
        }

        if (!request.IsSystem)
        {
            throw new UnauthorizedAccessException("Only the system can stop this auction.");
        }

        await auction.StopAuction(_clock.UtcNow);

        if (auction.Bids.Any())
        {
            lot.Sold();
        }
        else
        {
            lot.MarkUnsold();
        }

        await _transactionRunner.ExecuteAsync(async ct =>
        {
            await _lotRepository.SaveLotAsync(lot, ct);
            await _auctionRepository.SaveAuctionAsync(auction, ct);
        }, cancellationToken);
        await _domainEventHandler.DispatchAsync(auction.DomainEvents, cancellationToken);
        auction.ClearDomainEvents();
    }
}
