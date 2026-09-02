using Relora.Orders.Application.Requests;
using Relora.Orders.Application.Validators;
using Relora.Payments.Domain;
using Relora.Payments.Domain.Enums;

using Xunit;

namespace Relora.Domain.Tests;

public sealed class PaymentAndValidationTests
{
    [Fact]
    public void PaidPayment_CanBeRefundedOnce()
    {
        var payment = Payment.Create(Guid.NewGuid(), Guid.NewGuid(), 55, "eur", "cs_test_1");

        payment.MarkPaid("pi_test_1", DateTime.UtcNow);
        payment.MarkRefunded("re_test_1", DateTime.UtcNow);
        payment.MarkRefunded("re_test_1", DateTime.UtcNow);

        Assert.Equal("EUR", payment.Currency);
        Assert.Equal(PaymentStatus.Refunded, payment.Status);
        Assert.Equal("re_test_1", payment.StripeRefundId);
    }

    [Fact]
    public void PaymentCreate_RejectsEmptyStripeSession()
    {
        Assert.Throws<ArgumentException>(() => Payment.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            10,
            "EUR",
            " "));
    }

    [Fact]
    public void ShippingAddressValidator_ReturnsErrorsForInvalidAddress()
    {
        var validator = new ShippingAddressRequestValidator();
        var result = validator.Validate(new ShippingAddressRequest(
            "",
            "LVA",
            "",
            "",
            "",
            "",
            null,
            null));

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }
}
