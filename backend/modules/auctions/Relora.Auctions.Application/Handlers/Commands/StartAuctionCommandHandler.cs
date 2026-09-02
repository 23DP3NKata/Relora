using Relora.Auctions.Application.Commands;
using Relora.Auctions.Application.Interfaces;
using Relora.Items.Application.Interfaces;
using Relora.Shared.Domain.Enums;
using Relora.Shared.Domain.Time;
using Relora.Shared.Domain.Persistence;
using Relora.Shared.Infrastructure.Interfaces;

using MediatR;

namespace Relora.Auctions.Application.Handlers.Commands;

/// <summary>
/// Represents the start auction command handler class.
/// </summary>
public sealed class StartAuctionCommandHandler : IRequestHandler<StartAuctionCommand>
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly ILotRepository _lotRepository;
    private readonly IDomainEventDispatcher _domainEventHandler;
    private readonly IClock _clock;
    private readonly ITransactionRunner _transactionRunner;

    /// <summary>
    /// Initializes a new instance of the <see cref="StartAuctionCommandHandler"/> class.
    /// </summary>
    /// <param name="auctionRepository">Auction repository.</param>
    /// <param name="clock">Clock.</param>
    /// <param name="domainEventDispatcher">Domain event dispatcher.</param>
    public StartAuctionCommandHandler(
        IAuctionRepository auctionRepository,
        IClock clock, IDomainEventDispatcher domainEventDispatcher,
        ILotRepository lotRepository,
        ITransactionRunner transactionRunner)
    {
        _auctionRepository = auctionRepository;
        _clock = clock;
        _domainEventHandler = domainEventDispatcher;
        _lotRepository = lotRepository;
        _transactionRunner = transactionRunner;
    }
    /// <summary>
    /// Handles the operation.
    /// </summary>
    /// <param name="request">Input data for the operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Handle(StartAuctionCommand request, CancellationToken cancellationToken)
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

        if (!request.IsAdmin && lot.SellerId != request.UserId)
        {
            throw new UnauthorizedAccessException("Only the seller can start this auction.");
        }

        if (lot.Status != LotStatus.Published && lot.Status != LotStatus.Unsold)
        {
            throw new InvalidOperationException(
                "Auction can only be started for a published or unsold lot.");
        }

        auction.StartAuction(_clock.UtcNow, MapDuration(request.Duration));

        lot.List();
        await _transactionRunner.ExecuteAsync(async ct =>
        {
            await _lotRepository.SaveLotAsync(lot, ct);
            await _auctionRepository.SaveAuctionAsync(auction, ct);
        }, cancellationToken);
        await _domainEventHandler.DispatchAsync(auction.DomainEvents, cancellationToken);
        auction.ClearDomainEvents();
    }

    /// <summary>
    /// Performs the map duration operation.
    /// </summary>
    /// <param name="option">Option.</param>
    /// <returns>The operation result.</returns>
    private TimeSpan MapDuration(AuctionDurationOption option)
    {
        return option switch
        {
            AuctionDurationOption.Flash1Hour => TimeSpan.FromHours(1),
            AuctionDurationOption.Flash6Hours => TimeSpan.FromHours(6),
            AuctionDurationOption.Classic3Days => TimeSpan.FromDays(3),
            AuctionDurationOption.Classic7Days => TimeSpan.FromDays(7),
            AuctionDurationOption.Classic14Days => TimeSpan.FromDays(14),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
