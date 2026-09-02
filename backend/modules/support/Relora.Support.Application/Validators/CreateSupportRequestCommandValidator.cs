using Relora.Support.Application.Commands;

using FluentValidation;

namespace Relora.Support.Application.Validators;

public sealed class CreateSupportRequestCommandValidator : AbstractValidator<CreateSupportRequestCommand>
{
    public CreateSupportRequestCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Message).NotEmpty().MaximumLength(5000);
    }
}
