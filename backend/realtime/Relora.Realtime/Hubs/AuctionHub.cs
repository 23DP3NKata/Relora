using Microsoft.AspNetCore.SignalR;

namespace Relora.Realtime.Hubs;

/// <summary>
/// Represents the auction hub class.
/// </summary>
public sealed class AuctionHub : Hub
{
    public static string GroupName(Guid auctionId)
    {
        if (auctionId == Guid.Empty)
        {
            throw new ArgumentException("Auction id is required.", nameof(auctionId));
        }

        return $"auction:{auctionId:N}";
    }

    public static string LotGroupName(Guid lotId)
    {
        if (lotId == Guid.Empty)
        {
            throw new ArgumentException("Lot id is required.", nameof(lotId));
        }

        return $"lot:{lotId:N}";
    }

    public Task JoinAuction(Guid auctionId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, GroupName(auctionId));
    }

    public Task LeaveAuction(Guid auctionId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(auctionId));
    }

    public Task JoinLot(Guid lotId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, LotGroupName(lotId));
    }

    public Task LeaveLot(Guid lotId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, LotGroupName(lotId));
    }
}
