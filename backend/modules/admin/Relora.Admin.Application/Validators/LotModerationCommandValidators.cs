using Relora.Admin.Application.Commands;

using FluentValidation;

namespace Relora.Admin.Application.Validators;

public sealed class AcceptLotCommandValidator : AbstractValidator<AcceptLotCommand>
{
    public AcceptLotCommandValidator()
    {
        RuleFor(x => x.lotId).NotEmpty();
        RuleFor(x => x.adminId).NotEmpty();
    }
}

public sealed class RejectLotCommandValidator : AbstractValidator<RejectLotCommand>
{
    public RejectLotCommandValidator()
    {
        RuleFor(x => x.lotId).NotEmpty();
        RuleFor(x => x.adminId).NotEmpty();
        RuleFor(x => x.reason)
            .NotEmpty()
            .MaximumLength(2000);
    }
}

public sealed class DeleteLotCommandValidator : AbstractValidator<DeleteLotCommand>
{
    public DeleteLotCommandValidator()
    {
        RuleFor(x => x.lotId).NotEmpty();
        RuleFor(x => x.adminId).NotEmpty();
    }
}
