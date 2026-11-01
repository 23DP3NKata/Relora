using Relora.Orders.Application.Requests;
using Relora.Orders.Application.Validators;

using Xunit;

namespace Relora.Domain.Tests;

public sealed class ShippingAddressValidationTests
{
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
