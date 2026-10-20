namespace Relora.Orders.Domain.Enums;

/// <summary>
/// Represents the order status enum.
/// </summary>
public enum OrderStatus
{
    PendingPayment,
    PaymentExpired,
    Paid,
    AwaitingShipment,
    Shipped,
    Delivered,
    Completed,
    Cancelled,
    Disputed
}
