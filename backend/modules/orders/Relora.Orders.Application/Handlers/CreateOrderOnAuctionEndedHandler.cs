using Relora.Auctions.Domain.Events;
using Relora.Orders.Application.Interfaces;
using Relora.Orders.Domain;
using Relora.Shared.Domain.Time;
using Relora.Items.Application.Interfaces;

using MediatR;

namespace Relora.Orders.Application.Handlers;

/// <summary>
/// Represents the create order on auction ended handler class.
/// </summary>
public sealed class CreateOrderOnAuctionEndedHandler : INotificationHandler<AuctionEndedDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILotRepository _lotRepository;
    private readonly IClock _clock;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateOrderOnAuctionEndedHandler"/> class.
    /// </summary>
    /// <param name="orderRepository">Order repository.</param>
    /// <param name="clock">Clock.</param>
    /// <param name="lotRepository">Lot repository.</param>
    public CreateOrderOnAuctionEndedHandler(IOrderRepository orderRepository, IClock clock, ILotRepository lotRepository)
    {
        _orderRepository = orderRepository;
        _clock = clock;
        _lotRepository = lotRepository;
    }

    /// <summary>
    /// Handles the operation.
    /// </summary>
    /// <param name="endedDomainEvent">Ended domain event.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Handle(AuctionEndedDomainEvent endedDomainEvent, CancellationToken cancellationToken)
    {
        if (endedDomainEvent.WinnerId == null || endedDomainEvent.WinningMoney is null)
        {
            return;
        }

        if (await _orderRepository.ExistsForAuctionAsync(endedDomainEvent.AuctionId, cancellationToken))
        {
            throw new InvalidOperationException("Order for this auction already exists.");
        }

        var lot = await _lotRepository.GetLotById(endedDomainEvent.LotId, cancellationToken);

        if (lot == null)
        {
            throw new InvalidOperationException("Lot not found.");
        }

        var order = Order.Create
        (
            endedDomainEvent.AuctionId, lot.SellerId,
            endedDomainEvent.WinnerId.Value, endedDomainEvent.WinningMoney.Amount,
            endedDomainEvent.WinningMoney.Currency, _clock.UtcNow.Add(Order.PaymentWindow),
            lot.ShippingPrice,
            lot.ShippingCurrency,
            lot.ShippingOriginCountry,
            lot.ShipsToCountries,
            lot.ShippingHandlingDays
        );

        await _orderRepository.AddOrderAsync(order, cancellationToken);
        await _orderRepository.UpdateOrderAsync(order, cancellationToken);
    }
}
