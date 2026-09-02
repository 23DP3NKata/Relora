using Relora.Identity.Application.Commands;
using FluentValidation;

namespace Relora.Identity.Application.Validators;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.password)
            .NotEmpty()
            .MinimumLength(6);
    }
}