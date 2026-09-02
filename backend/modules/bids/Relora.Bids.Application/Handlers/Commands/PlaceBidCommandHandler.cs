using System.Threading;

using Relora.Auctions.Application.Interfaces;
using Relora.Bids.Application.Commands;
using Relora.Items.Application.Interfaces;
using Relora.Shared.Domain.Time;
using Relora.Shared.Domain.ValueObjects;
using Relora.Shared.Infrastructure.Interfaces;

using MediatR;

namespace Relora.Bids.Application.Handlers.Commands;

/// <summary>
/// Represents the place bid command handler class.
/// </summary>
public sealed class PlaceBidCommandHandler
    : IRequestHandler<PlaceBidCommand>
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly ILotRepository _lotRepository;
    private readonly IDomainEventDispatcher _domainEventHandler;
    private readonly IClock _clock;

    public PlaceBidCommandHandler(
        IAuctionRepository auctionRepository,
        IDomainEventDispatcher domainEventHandler,
        IClock clock,
        ILotRepository lotRepository)
    {
        _auctionRepository = auctionRepository;
        _domainEventHandler = domainEventHandler;
        _clock = clock;
        _lotRepository = lotRepository;
    }

    public async Task Handle(PlaceBidCommand request, CancellationToken cancellationToken)
    {
        if (request.BidderId == Guid.Empty)
        {
            throw new ArgumentException("BidderId is required.", nameof(request.BidderId));
        }

        var auction = await _auctionRepository.GetAuctionById(request.AuctionId, cancellationToken);

        if (auction == null)
        {
            throw new KeyNotFoundException($"Auction {request.AuctionId} not found.");
        }

        var lot = await _lotRepository.GetLotById(auction.LotId, cancellationToken);

        if (lot?.SellerId == request.BidderId)
        {
           throw new InvalidOperationException("You cannot bid on your own auction.");
        }

        auction.PlaceBid(
            Guid.NewGuid(),
            request.BidderId,
            new Money(request.Amount, request.Currency),
            _clock.UtcNow
        );

        await _auctionRepository.SaveAuctionAsync(auction, cancellationToken);
        await _domainEventHandler.DispatchAsync(auction.DomainEvents, cancellationToken);

        auction.ClearDomainEvents();
    }
}
