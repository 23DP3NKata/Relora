using Relora.Auctions.Domain;
using Relora.Shared.Domain.Enums;
using Relora.Shared.Domain.ValueObjects;

using Xunit;

namespace Relora.Domain.Tests;

public sealed class AuctionTests
{
    [Fact]
    public async Task StopAuctionWithoutBids_MarksAuctionUnsold_AndItCanBeRestarted()
    {
        var auction = new Auction(Guid.NewGuid(), new Money(10, "EUR"));
        var startedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc);

        auction.StartAuction(startedAt, TimeSpan.FromDays(3));
        await auction.StopAuction(startedAt.AddDays(3));

        Assert.Equal(AuctionStatus.Unsold, auction.Status);

        auction.StartAuction(startedAt.AddDays(4), TimeSpan.FromDays(7));

        Assert.Equal(AuctionStatus.Active, auction.Status);
        Assert.Equal(startedAt.AddDays(11), auction.EndDate);
    }

    [Fact]
    public void PlaceBidNearEnd_ExtendsAuctionEndTime()
    {
        var auction = new Auction(Guid.NewGuid(), new Money(10, "EUR"));
        var startedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc);
        auction.StartAuction(startedAt, TimeSpan.FromHours(1));

        var bidTime = startedAt.AddHours(1).AddSeconds(-5);
        auction.PlaceBid(Guid.NewGuid(), Guid.NewGuid(), new Money(11, "EUR"), bidTime);

        Assert.Equal(bidTime.AddSeconds(35), auction.EndDate);
        Assert.Equal(11, auction.CurrentPrice.Amount);
    }
}
