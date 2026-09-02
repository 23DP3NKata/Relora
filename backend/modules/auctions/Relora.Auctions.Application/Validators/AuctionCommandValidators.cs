using Relora.Auctions.Application.Commands;

using FluentValidation;

namespace Relora.Auctions.Application.Validators;

public sealed class CreateAuctionCommandValidator : AbstractValidator<CreateAuctionCommand>
{
    public CreateAuctionCommandValidator()
    {
        RuleFor(x => x.LotId).NotEmpty();
        RuleFor(x => x.UserId)
            .NotEmpty()
            .When(x => !x.IsAdmin);
    }
}

public sealed class StartAuctionCommandValidator : AbstractValidator<StartAuctionCommand>
{
    public StartAuctionCommandValidator()
    {
        RuleFor(x => x.AuctionId).NotEmpty();
        RuleFor(x => x.Duration).IsInEnum();
        RuleFor(x => x.UserId)
            .NotEmpty()
            .When(x => !x.IsAdmin);
    }
}

public sealed class StopAuctionCommandValidator : AbstractValidator<StopAuctionCommand>
{
    public StopAuctionCommandValidator()
    {
        RuleFor(x => x.AuctionId).NotEmpty();
        RuleFor(x => x.UserId)
            .NotEmpty()
            .When(x => !x.IsAdmin && !x.IsSystem);
    }
}
