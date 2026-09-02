using Relora.Identity.Application.Interfaces;
using Relora.Identity.Infrastructure.Claims;
using Relora.Orders.Application.Interfaces;
using Relora.Payments.Application.Interfaces;
using Relora.Payments.Domain;
using Relora.Payments.Domain.Enums;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

using Stripe;

namespace Relora.Payments.API;

[ApiController]
[Route("api/seller/stripe")]
[Authorize]
public class SellerStripeController(
    IUserRepository userRepository,
    ISellerPaymentAccountRepository sellerPaymentAccountRepository,
    IConfiguration configuration
    ) : ControllerBase
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ISellerPaymentAccountRepository _sellerPaymentAccountRepository = sellerPaymentAccountRepository;
    private readonly string _publicSiteUrl = configuration["PublicSite:Url"]?.TrimEnd('/')
        ?? throw new InvalidOperationException("PublicSite:Url must be configured.");

    [HttpPost("onboarding")]
    public async Task<IActionResult> CreateOnboarding(CancellationToken cancellationToken)
    {
        var sellerId = User.Claims.GetUserId();

        var seller = await _userRepository.GetUserByIdAsync(sellerId);

        if (seller == null)
        {
            return NotFound("Seller not found.");
        }

        var paymentAccount = await _sellerPaymentAccountRepository.GetBySellerIdAsync(sellerId, cancellationToken);

        if (paymentAccount == null)
        {
            var accountService = new AccountService();

            var account = await accountService.CreateAsync(new AccountCreateOptions
            {
                Type = "express",
                Country = "LV",
                Email = seller.Email,
                Capabilities = new AccountCapabilitiesOptions
                {
                    Transfers = new AccountCapabilitiesTransfersOptions
                    {
                        Requested = true
                    }
                }
            }, cancellationToken: cancellationToken);

            paymentAccount = SellerPaymentAccount.Create(sellerId, account.Id);

            await _sellerPaymentAccountRepository.AddSellerPaymentAccountAsync(paymentAccount, cancellationToken);
        }

        var accountLinkService = new AccountLinkService();

        var accountLink = await accountLinkService.CreateAsync(new AccountLinkCreateOptions
        {
            Account = paymentAccount.StripeConnectedAccountId,
            RefreshUrl = $"{_publicSiteUrl}/seller/stripe/refresh",
            ReturnUrl = $"{_publicSiteUrl}/seller/stripe/return",
            Type = "account_onboarding"
        }, cancellationToken: cancellationToken);

        await _sellerPaymentAccountRepository.UpdateSellerPaymentAccountAsync(paymentAccount, cancellationToken);

        return Ok(new { url = accountLink.Url });
    }


    [HttpGet("status")]
    public async Task<IActionResult> GetStripeStatus(CancellationToken cancellationToken)
    {
        var sellerId = User.Claims.GetUserId();

        var paymentAccount = await _sellerPaymentAccountRepository
            .GetBySellerIdAsync(sellerId, cancellationToken);

        if (paymentAccount == null)
        {
            return Ok(new
            {
                connected = false,
                ready = false
            });
        }

        var accountService = new AccountService();

        var stripeAccount = await accountService.GetAsync(
            paymentAccount.StripeConnectedAccountId,
            cancellationToken: cancellationToken
        );

        paymentAccount.UpdateStripeStatus(
            detailsSubmitted: stripeAccount.DetailsSubmitted,
            chargesEnabled: stripeAccount.ChargesEnabled,
            payoutsEnabled: stripeAccount.PayoutsEnabled
        );

        await _sellerPaymentAccountRepository.UpdateSellerPaymentAccountAsync(paymentAccount, cancellationToken);

        return Ok(new
        {
            connected = true,
            ready = paymentAccount.Status == SellerPaymentAccountStatus.Ready,
            detailsSubmitted = paymentAccount.DetailsSubmitted,
            chargesEnabled = paymentAccount.ChargesEnabled,
            payoutsEnabled = paymentAccount.PayoutsEnabled
        });
    }
}
