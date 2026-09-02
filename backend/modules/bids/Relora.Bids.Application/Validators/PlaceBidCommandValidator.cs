using Relora.Bids.Application.Commands;

using FluentValidation;

namespace Relora.Bids.Application.Validators;

public sealed class PlaceBidCommandValidator : AbstractValidator<PlaceBidCommand>
{
    public PlaceBidCommandValidator()
    {
        RuleFor(x => x.AuctionId).NotEmpty();
        RuleFor(x => x.BidderId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency)
            .NotEmpty()
            .Matches("^[A-Z]{3}$")
            .WithMessage("Currency must be a three-letter uppercase ISO code.");
    }
}
