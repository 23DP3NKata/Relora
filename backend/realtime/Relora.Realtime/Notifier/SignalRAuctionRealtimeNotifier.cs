using Microsoft.AspNetCore.SignalR;
using Relora.Auctions.Application.Interfaces;
using Relora.Realtime.Hubs;

namespace Relora.Realtime.Notifier;

/// <summary>
/// Represents the signal r auction realtime notifier class.
/// </summary>
public sealed class SignalRAuctionRealtimeNotifier(IHubContext<AuctionHub> hubContext) : IAuctionRealtimeNotifier
{
    private readonly IHubContext<AuctionHub> _hubContext = hubContext;

    /// <summary>
    /// Performs the bid placed operation.
    /// </summary>
    /// <param name="auctionId">Identifier of auction.</param>
    /// <param name="bidderId">Identifier of bidder.</param>
    /// <param name="amount">Amount.</param>
    /// <param name="currency">Currency.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task BidPlaced(Guid auctionId, Guid bidderId, decimal amount, string currency)
    {
        return _hubContext.Clients.Group(AuctionHub.GroupName(auctionId)).SendAsync
        (
            "BidPlaced",
            new { auctionId, bidderId, amount, currency }
        );
    }

    /// <summary>
    /// Performs the auction ended operation.
    /// </summary>
    /// <param name="auctionId">Identifier of auction.</param>
    /// <param name="winnerId">Identifier of winner.</param>
    /// <param name="winningBidId">Identifier of winning bid.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task AuctionEnded(Guid auctionId, Guid? winnerId, Guid? winningBidId)
    {
        return _hubContext.Clients.Group(AuctionHub.GroupName(auctionId)).SendAsync
        (
            "AuctionEnded",
            new { auctionId, winnerId, winningBidId }
        );
    }

    /// <summary>
    /// Performs the auction started operation.
    /// </summary>
    /// <param name="auctionId">Identifier of auction.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task AuctionStarted(Guid auctionId, Guid? lotId)
    {
        var payload = new { auctionId, lotId };
        var auctionNotification = _hubContext.Clients.Group(AuctionHub.GroupName(auctionId)).SendAsync
        (
            "AuctionStarted",
            payload
        );

        if (!lotId.HasValue || lotId.Value == Guid.Empty)
        {
            return auctionNotification;
        }

        var lotNotification = _hubContext.Clients.Group(AuctionHub.LotGroupName(lotId.Value)).SendAsync
        (
            "AuctionStarted",
            payload
        );

        return Task.WhenAll(auctionNotification, lotNotification);
    }
}
