using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Relora.Payments.Domain.Enums;

namespace Relora.Payments.Domain;

public class SellerPaymentAccount
{
    public Guid Id { get; set; }

    public Guid SellerId { get; private set; }

    public string Provider { get; private set; } = "Stripe";

    public string StripeConnectedAccountId { get; private set; } = null!;

    public bool DetailsSubmitted { get; private set; }
    public bool ChargesEnabled { get; private set; }
    public bool PayoutsEnabled { get; private set; }

    public SellerPaymentAccountStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public SellerPaymentAccount() { }

    public static SellerPaymentAccount Create(Guid sellerId, string stripeConnectedAccountId)
    {
        return new SellerPaymentAccount
        {
            Id = Guid.NewGuid(),
            SellerId = sellerId,
            StripeConnectedAccountId = stripeConnectedAccountId,
            Status = SellerPaymentAccountStatus.OnboardingRequired,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateStripeStatus(bool detailsSubmitted, bool chargesEnabled, bool payoutsEnabled)
    {
        DetailsSubmitted = detailsSubmitted;
        ChargesEnabled = chargesEnabled;
        PayoutsEnabled = payoutsEnabled;
        UpdatedAt = DateTime.UtcNow;

        Status = payoutsEnabled
            ? SellerPaymentAccountStatus.Ready
            : SellerPaymentAccountStatus.OnboardingRequired;
    }
}
