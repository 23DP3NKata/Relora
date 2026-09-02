using Relora.Orders.Application.Commands;
using Relora.Orders.Application.Requests;

using FluentValidation;

namespace Relora.Orders.Application.Validators;

public sealed class ConfirmOrderReceivedCommandValidator : AbstractValidator<ConfirmOrderReceivedCommand>
{
    public ConfirmOrderReceivedCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.BuyerId).NotEmpty();
    }
}

public sealed class MarkOrderShippedCommandValidator : AbstractValidator<MarkOrderShippedCommand>
{
    public MarkOrderShippedCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.SellerId).NotEmpty();
        RuleFor(x => x.CarrierName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.TrackingNumber).NotEmpty().MaximumLength(120);
    }
}

public sealed class OpenOrderDisputeCommandValidator : AbstractValidator<OpenOrderDisputeCommand>
{
    public OpenOrderDisputeCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.BuyerId).NotEmpty();
        RuleFor(x => x.Reason).IsInEnum();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.EvidenceKeys)
            .Must(keys => keys is null || keys.Count <= 10)
            .WithMessage("A dispute can contain at most 10 evidence files.");
        RuleFor(x => x.EvidenceKeys)
            .Must(keys => keys is null || keys.Distinct(StringComparer.Ordinal).Count() == keys.Count)
            .WithMessage("Evidence files must be unique.");

        RuleForEach(x => x.EvidenceKeys)
            .NotEmpty()
            .MaximumLength(500)
            .When(x => x.EvidenceKeys is not null);
    }
}

public sealed class ReportOrderNotDeliveredCommandValidator : AbstractValidator<ReportOrderNotDeliveredCommand>
{
    public ReportOrderNotDeliveredCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.BuyerId).NotEmpty();
        RuleFor(x => x.Reason)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Reason));
    }
}

public sealed class ResolveOrderDisputeCommandValidator : AbstractValidator<ResolveOrderDisputeCommand>
{
    public ResolveOrderDisputeCommandValidator()
    {
        RuleFor(x => x.DisputeId).NotEmpty();
        RuleFor(x => x.AdminId).NotEmpty();
        RuleFor(x => x.Decision).IsInEnum();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(2000);
    }
}

public sealed class CreateOrderPaymentSessionCommandValidator : AbstractValidator<CreateOrderPaymentSessionCommand>
{
    public CreateOrderPaymentSessionCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();

        When(x => x.ShippingAddress is not null, () =>
        {
            RuleFor(x => x.ShippingAddress!.FullName).NotEmpty().MaximumLength(160);
            RuleFor(x => x.ShippingAddress!.CountryCode).Matches("^[A-Za-z]{2}$");
            RuleFor(x => x.ShippingAddress!.Country).NotEmpty().MaximumLength(100);
            RuleFor(x => x.ShippingAddress!.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.ShippingAddress!.PostalCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.ShippingAddress!.AddressLine1).NotEmpty().MaximumLength(240);
            RuleFor(x => x.ShippingAddress!.AddressLine2).MaximumLength(240);
            RuleFor(x => x.ShippingAddress!.Phone).MaximumLength(40);
        });
    }
}

public sealed class ShippingAddressRequestValidator : AbstractValidator<ShippingAddressRequest>
{
    public ShippingAddressRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(160);
        RuleFor(x => x.CountryCode).Matches("^[A-Za-z]{2}$");
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(30);
        RuleFor(x => x.AddressLine1).NotEmpty().MaximumLength(240);
        RuleFor(x => x.AddressLine2).MaximumLength(240);
        RuleFor(x => x.Phone).MaximumLength(40);
    }
}
