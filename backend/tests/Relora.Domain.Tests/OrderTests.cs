using Relora.Orders.Domain;
using Relora.Orders.Domain.Enums;

using Xunit;

namespace Relora.Domain.Tests;

public sealed class OrderTests
{
    [Fact]
    public void MarkAsPaid_SetsShippingDeadlineInBusinessDays()
    {
        var paidAt = new DateTime(2026, 7, 3, 12, 0, 0, DateTimeKind.Utc);
        var order = CreateOrder(paidAt.AddDays(3), shippingHandlingDays: 1);

        order.MarkAsPaid(paidAt);

        Assert.Equal(OrderStatus.AwaitingShipment, order.Status);
        Assert.Equal(new DateTime(2026, 7, 6, 12, 0, 0, DateTimeKind.Utc), order.ShipByUtc);
    }

    [Fact]
    public void Create_RejectsDifferentOrderAndShippingCurrencies()
    {
        Assert.Throws<ArgumentException>(() => Order.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            50,
            "EUR",
            DateTime.UtcNow.AddDays(3),
            5,
            "USD",
            "Latvia",
            "LV",
            3));
    }

    [Fact]
    public void ResolveDisputeWithRefund_MarksOrderRefunded()
    {
        var buyerId = Guid.NewGuid();
        var order = CreateOrder(DateTime.UtcNow.AddDays(3), buyerId: buyerId);
        order.MarkAsPaid(DateTime.UtcNow);

        var dispute = OrderDispute.Open(
            order,
            buyerId,
            OrderDisputeReason.ItemNotShipped,
            "The seller has not sent the item.",
            null,
            DateTime.UtcNow);

        dispute.Resolve(
            order,
            Guid.NewGuid(),
            OrderDisputeDecision.Refund,
            "Shipping deadline was missed.",
            DateTime.UtcNow);

        Assert.Equal(OrderDisputeStatus.Resolved, dispute.Status);
        Assert.Equal(OrderDisputeDecision.Refund, dispute.Decision);
        Assert.Equal(OrderStatus.Refunded, order.Status);
    }

    [Fact]
    public void OpenDisputeAfterCompletedOrder_IsRejected()
    {
        var buyerId = Guid.NewGuid();
        var order = CreateOrder(DateTime.UtcNow.AddDays(3), buyerId: buyerId);
        order.MarkAsPaid(DateTime.UtcNow);
        order.MarkAsShipped("DPD", "TRACK-1", DateTime.UtcNow);
        order.ConfirmReceived(DateTime.UtcNow);

        Assert.Throws<InvalidOperationException>(() => OrderDispute.Open(
            order,
            buyerId,
            OrderDisputeReason.ItemNotReceived,
            "The package was not received.",
            null,
            DateTime.UtcNow));
    }

    private static Order CreateOrder(DateTime paymentDeadline, Guid? buyerId = null, int shippingHandlingDays = 3)
    {
        return Order.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            buyerId ?? Guid.NewGuid(),
            50,
            "EUR",
            paymentDeadline,
            5,
            "EUR",
            "Latvia",
            "LV",
            shippingHandlingDays);
    }
}
