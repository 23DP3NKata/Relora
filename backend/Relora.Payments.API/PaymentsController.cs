using System.Text;

using Relora.Identity.Infrastructure.Claims;
using Relora.Orders.Application.Interfaces;
using Relora.Orders.Application.Requests;
using Relora.Payments.Application.Service;
using Relora.Orders.Domain.Enums;
using Relora.Orders.Infrastructure.Options;
using Relora.Orders.Infrastructure.Repository;
using Relora.Payments.Application.Interfaces;
using Relora.Payments.Domain;
using Relora.Shared.Domain.Time;
using Relora.Shared.Domain.Persistence;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;

namespace Relora.Payments.API;


[ApiController]
[Route("api/payments")]
public class PaymentsController(
    IOrderRepository orderRepository,
    IPaymentRepository paymentRepository,
    PaymentService paymentService,
    StripeOptions stripeOptions,
    IConfiguration configuration,
    IClock clock,
    ITransactionRunner transactions
    ) : ControllerBase
{
    private readonly IOrderRepository _ordersRepository = orderRepository;
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly PaymentService _paymentService = paymentService;
    private readonly StripeOptions _stripeOptions = stripeOptions;
    private readonly string _publicSiteUrl = configuration["PublicSite:Url"]?.TrimEnd('/')
        ?? throw new InvalidOperationException("PublicSite:Url must be configured.");
    private readonly IClock _clock = clock;
    private readonly ITransactionRunner _transactions = transactions;

    [HttpPost]
    [Route("webhook")]
    /// <summary>
    /// Performs the stripe webhook operation.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<IActionResult> StripeWebhook(CancellationToken cancellationToken)
    {
        string json;

        using (var reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8))
        {
            json = await reader.ReadToEndAsync(cancellationToken);
        }

        var signatureHeader = Request.Headers["Stripe-Signature"];

        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                signatureHeader,
                _stripeOptions.WebhookSecret);
        }
        catch (StripeException)
        {
            return BadRequest("Invalid Stripe signature.");
        }
        catch (Exception)
        {
            return BadRequest("Invalid webhook payload.");
        }

        switch (stripeEvent.Type)
        {
            case "checkout.session.completed":
            case "checkout.session.async_payment_succeeded":
                {
                    var session = stripeEvent.Data.Object as Session;

                    if (session is null)
                    {
                        return BadRequest("Session object not found.");
                    }

                    await _paymentService.HandleSuccessfulCheckoutSession(session, cancellationToken);
                    break;
                }

            case "checkout.session.async_payment_failed":
                {
                    var session = stripeEvent.Data.Object as Session;

                    if (session is null)
                    {
                        return BadRequest("Session object not found.");
                    }

                    await _paymentService.HandleFailedCheckoutSession(session, cancellationToken);
                    break;
                }

            case "checkout.session.expired":
                {
                    var session = stripeEvent.Data.Object as Session;

                    if (session is null)
                    {
                        return BadRequest("Session object not found.");
                    }

                    await _paymentService.HandleExpiredCheckoutSession(session, cancellationToken);
                    break;
                }
        }

        return Ok();
    }

    [HttpPost("/orders/{orderId:guid}/checkout")]
    [Authorize]
    public async Task<IActionResult> CreateCheckout(
        Guid orderId,
        [FromBody] ShippingAddressRequest? shippingAddress,
        CancellationToken cancellationToken)
    {
        var order = await _ordersRepository.GetOrderByIdAsync(orderId, cancellationToken);

        if (order == null)
        {
            return NotFound();
        }

        if (order.Status != OrderStatus.PendingPayment)
        {
            return BadRequest("Order is not awaiting payment.");
        }

        var now = _clock.UtcNow;
        if (now >= order.PaymentDeadlineUtc)
        {
            return BadRequest("Payment deadline has expired.");
        }

        if (order.BuyerId != User.Claims.GetUserId())
        {
            return Forbid();
        }

        if (shippingAddress is not null)
        {
            try
            {
                order.SetShippingAddress(
                    shippingAddress.FullName,
                    shippingAddress.CountryCode,
                    shippingAddress.Country,
                    shippingAddress.City,
                    shippingAddress.PostalCode,
                    shippingAddress.AddressLine1,
                    shippingAddress.AddressLine2,
                    shippingAddress.Phone);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        if (!order.HasShippingAddress)
        {
            return BadRequest("Shipping address is required before payment.");
        }

        var sessionService = new Stripe.Checkout.SessionService();

        var currentSessionId = order.StripeCheckoutSessionId;
        if (!string.IsNullOrWhiteSpace(currentSessionId))
        {
            var existingSession = await sessionService.GetAsync(
                currentSessionId,
                cancellationToken: cancellationToken);

            if (string.Equals(existingSession.Status, "open", StringComparison.OrdinalIgnoreCase) &&
                existingSession.ExpiresAt > now &&
                !string.IsNullOrWhiteSpace(existingSession.Url))
            {
                await _ordersRepository.UpdateOrderAsync(order, cancellationToken);
                return Ok(new { url = existingSession.Url });
            }

            order.ClearStripeSession(currentSessionId);
            await _ordersRepository.UpdateOrderAsync(order, cancellationToken);
        }

        var checkoutExpiresAt = order.PaymentDeadlineUtc < now.AddHours(24)
            ? order.PaymentDeadlineUtc
            : now.AddHours(24);

        if (checkoutExpiresAt <= now.AddMinutes(30))
        {
            return BadRequest("Payment deadline is too close to start a new checkout session.");
        }

        var session = await sessionService.CreateAsync(new Stripe.Checkout.SessionCreateOptions
        {
            Mode = "payment",
            SuccessUrl = $"{_publicSiteUrl}/orders/payment-success?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{_publicSiteUrl}/orders/payment-cancelled",
            ExpiresAt = checkoutExpiresAt,

            ClientReferenceId = order.Id.ToString(),

            Metadata = new Dictionary<string, string>
            {
                ["orderId"] = order.Id.ToString(),
                ["buyerId"] = order.BuyerId.ToString(),
                ["sellerId"] = order.SellerId.ToString()
            },

            PaymentIntentData = new Stripe.Checkout.SessionPaymentIntentDataOptions
            {
                TransferGroup = $"ORDER_{order.Id}",
                Metadata = new Dictionary<string, string>
                {
                    ["orderId"] = order.Id.ToString()
                }
            },

        LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
        {
            new()
            {
                Quantity = 1,
                PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                {
                    Currency = order.Currency.ToLowerInvariant(),
                    UnitAmount = ToCents(order.TotalPrice),
                    ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                    {
                        Name = $"Relora order #{order.Id}"
                    }
                }
            }
        }
        }, new RequestOptions
        {
            IdempotencyKey = order.GetCheckoutIdempotencyKey()
        }, cancellationToken);

        order.SetStripeSession(session.Id);
        await _transactions.ExecuteAsync(async ct =>
        {
            if (await _paymentRepository.GetByStripeSessionIdAsync(session.Id, ct) is null)
            {
                var payment = Payment.Create(
                    order.Id,
                    order.BuyerId,
                    order.TotalPrice,
                    order.Currency,
                    session.Id
                );

                await _paymentRepository.AddPaymentAsync(payment, ct);
            }

            await _ordersRepository.UpdateOrderAsync(order, ct);
        }, cancellationToken);

        return Ok(new { url = session.Url });
    }

    private static long ToCents(decimal amount)
    {
        return (long)Math.Round(amount * 100, MidpointRounding.AwayFromZero);
    }

}
