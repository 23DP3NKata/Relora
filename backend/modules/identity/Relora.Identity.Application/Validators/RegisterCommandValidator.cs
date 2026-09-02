using Relora.Identity.Application.Commands;
using FluentValidation;

namespace Relora.Identity.Application.Validators;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(20);

        RuleFor(x => x.email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.password)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(x => x.confirmPassword)
            .NotEmpty()
            .MinimumLength(6)
            .Equal(x => x.password)
            .WithMessage("Passwords do not match");
    }
}
