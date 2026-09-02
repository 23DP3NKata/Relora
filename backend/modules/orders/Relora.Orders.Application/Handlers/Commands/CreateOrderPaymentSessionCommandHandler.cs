using Relora.Orders.Application.Commands;
using Relora.Orders.Application.Interfaces;
using Relora.Orders.Domain.Enums;
using Relora.Orders.Infrastructure.Options;
using Relora.Shared.Domain.Time;

using MediatR;

using Microsoft.Extensions.Options;

using Stripe.Checkout;

/// <summary>
/// Represents the create order payment session command handler class.
/// </summary>
public sealed class CreateOrderPaymentSessionCommandHandler
    : IRequestHandler<CreateOrderPaymentSessionCommand, CreateOrderPaymentSessionResult>
{
    private readonly StripeOptions _stripeOptions;
    private readonly IOrderRepository _orders;
    private readonly SessionService _sessionService;
    private readonly IClock _clock;

    public CreateOrderPaymentSessionCommandHandler(
        IOptions<StripeOptions> stripeOptions,
        IOrderRepository orders,
        SessionService sessionService,
        IClock clock)
    {
        _stripeOptions = stripeOptions.Value;
        _orders = orders;
        _sessionService = sessionService;
        _clock = clock;
    }

    public async Task<CreateOrderPaymentSessionResult> Handle(
        CreateOrderPaymentSessionCommand command,
        CancellationToken ct)
    {
        var order = await _orders.GetOrderByIdAsync(command.OrderId, ct);

        if (order is null)
        {
            throw new KeyNotFoundException($"Order {command.OrderId} not found.");
        }

        if (order.BuyerId != command.UserId)
        {
            throw new UnauthorizedAccessException("This order does not belong to the user.");
        }

        if (order.Status != OrderStatus.PendingPayment)
        {
            throw new InvalidOperationException("Order is not available for payment.");
        }

        var now = _clock.UtcNow;
        if (now >= order.PaymentDeadlineUtc)
        {
            throw new InvalidOperationException("Payment deadline has expired.");
        }

        if (command.ShippingAddress is not null)
        {
            order.SetShippingAddress(
                command.ShippingAddress.FullName,
                command.ShippingAddress.CountryCode,
                command.ShippingAddress.Country,
                command.ShippingAddress.City,
                command.ShippingAddress.PostalCode,
                command.ShippingAddress.AddressLine1,
                command.ShippingAddress.AddressLine2,
                command.ShippingAddress.Phone);
        }

        if (!order.HasShippingAddress)
        {
            throw new InvalidOperationException("Shipping address is required before payment.");
        }

        var amountInCents = (long)Math.Round(order.TotalPrice * 100m, MidpointRounding.AwayFromZero);
        var checkoutExpiresAt = order.PaymentDeadlineUtc < now.AddHours(24)
            ? order.PaymentDeadlineUtc
            : now.AddHours(24);

        if (checkoutExpiresAt <= now.AddMinutes(30))
        {
            throw new InvalidOperationException("Payment deadline is too close to start a new checkout session.");
        }

        var options = new SessionCreateOptions
        {
            Mode = "payment",
            SuccessUrl = _stripeOptions.SuccessUrl + "?session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = _stripeOptions.CancelUrl,
            ExpiresAt = checkoutExpiresAt,
            ClientReferenceId = order.Id.ToString(),
            Metadata = new Dictionary<string, string>
            {
                ["orderId"] = order.Id.ToString(),
                ["buyerId"] = order.BuyerId.ToString()
            },
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = order.Currency.ToLowerInvariant(),
                        UnitAmount = amountInCents,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Auction item {order.AuctionId}"
                        }
                    }
                }
            }
        };

        var session = await _sessionService.CreateAsync(options, cancellationToken: ct);

        if (string.IsNullOrWhiteSpace(session.Url))
        {
            throw new InvalidOperationException("Stripe checkout session URL was not returned.");
        }

        order.SetStripeSession(session.Id);

        await _orders.UpdateOrderAsync(order, ct);

        return new CreateOrderPaymentSessionResult(session.Url);
    }
}
