using Relora.Items.Application.Commands;

using FluentValidation;

namespace Relora.Items.Application.Validators;

public sealed class EditLotCommandValidator : AbstractValidator<EditLotCommand>
{
    public EditLotCommandValidator()
    {
        RuleFor(x => x.id).NotEmpty();
        RuleFor(x => x.sellerId).NotEmpty();

        RuleFor(x => x.title)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(120);

        RuleFor(x => x.description)
            .NotEmpty()
            .MinimumLength(100)
            .MaximumLength(2000);

        RuleFor(x => x.brand)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(80);

        RuleFor(x => x.color)
            .MaximumLength(40)
            .When(x => !string.IsNullOrWhiteSpace(x.color));

        RuleFor(x => x.categoryId)
            .NotEqual(Guid.Empty)
            .When(x => x.categoryId.HasValue);

        RuleFor(x => x.department)
            .IsInEnum()
            .When(x => x.department.HasValue);

        RuleFor(x => x.primaryColorId)
            .NotEqual(Guid.Empty)
            .When(x => x.primaryColorId.HasValue);

        RuleFor(x => x.modelName)
            .MaximumLength(120)
            .When(x => !string.IsNullOrWhiteSpace(x.modelName));

        RuleFor(x => x.acquisitionYear)
            .InclusiveBetween(1900, DateTime.UtcNow.Year)
            .When(x => x.acquisitionYear.HasValue);

        RuleFor(x => x.productionYear)
            .InclusiveBetween(1800, DateTime.UtcNow.Year)
            .When(x => x.productionYear.HasValue);

        RuleFor(x => x)
            .Must(x => !x.acquisitionYear.HasValue ||
                       !x.productionYear.HasValue ||
                       x.acquisitionYear.Value >= x.productionYear.Value)
            .WithMessage("Acquisition year cannot be earlier than production year.");

        RuleFor(x => x.vintageNotes)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.vintageNotes));

        RuleFor(x => x.country)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.city)
            .MaximumLength(30);

        RuleFor(x => x.shippingPrice)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.price)
            .Must(price => price is not null && price.Amount > 0)
            .WithMessage("Price must be greater than zero.");

        RuleFor(x => x.price)
            .Must(price => price is not null && !string.IsNullOrWhiteSpace(price.Currency) && price.Currency.Length == 3)
            .WithMessage("Currency must be a three-letter ISO code.");

        RuleFor(x => x.category).IsInEnum();
        RuleFor(x => x.gender).IsInEnum();
        RuleFor(x => x.size).IsInEnum();
        RuleFor(x => x.condition).IsInEnum();

        RuleFor(x => x.age)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.style)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.shippingCurrency)
            .Matches("^[A-Z]{3}$")
            .WithMessage("Shipping currency must be a three-letter uppercase ISO code.");

        RuleFor(x => x.shippingOriginCountry)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.shipsToCountries)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.shippingHandlingDays)
            .InclusiveBetween(1, 30);

        RuleFor(x => x.photoKeys)
            .Must(keys => keys is null || keys.Count >= 5)
            .WithMessage("At least 5 photos are required.");

        RuleFor(x => x.photoKeys)
            .Must(keys => keys is null || keys.Count <= 15)
            .WithMessage("Maximum 15 photos are allowed.");

        RuleForEach(x => x.photoKeys)
            .NotEmpty()
            .MaximumLength(500)
            .When(x => x.photoKeys is not null);

        RuleFor(x => x.photoKeys)
            .Must(keys => keys is null || keys.Distinct(StringComparer.Ordinal).Count() == keys.Count)
            .WithMessage("Photo keys must be unique.");

        RuleFor(x => x.coverPhotoKey)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.coverPhotoKey));

        RuleFor(x => x)
            .Must(x => string.IsNullOrWhiteSpace(x.coverPhotoKey) ||
                       (x.photoKeys is not null && x.photoKeys.Contains(x.coverPhotoKey.Trim(), StringComparer.Ordinal)))
            .WithMessage("Cover photo must be one of the listing photos.");

        RuleFor(x => x.materials)
            .Must(materials => materials is null || materials.Count <= 3)
            .WithMessage("You can select up to 3 materials.");

        RuleFor(x => x.materials)
            .Must(materials => materials is null ||
                               materials.All(material =>
                                   material.MaterialId != Guid.Empty &&
                                   (!material.Percentage.HasValue ||
                                    (material.Percentage.Value >= 0 && material.Percentage.Value <= 100))))
            .WithMessage("Materials must have valid ids and percentages between 0 and 100.");

        RuleFor(x => x.materials)
            .Must(materials => materials is null ||
                                materials.Select(material => material.MaterialId).Distinct().Count() == materials.Count)
            .WithMessage("Material ids must be unique.");

        RuleFor(x => x.measurements)
            .Must(measurements => measurements is null ||
                                  measurements.All(measurement =>
                                      !string.IsNullOrWhiteSpace(measurement.Key) &&
                                      measurement.Value > 0 &&
                                      (string.IsNullOrWhiteSpace(measurement.Unit) ||
                                       measurement.Unit.Length <= 12)))
            .WithMessage("Measurements must have a key, positive value, and valid unit.");

        RuleFor(x => x.measurements)
            .Must(measurements => measurements is null ||
                                  measurements.Select(measurement => measurement.Key.Trim().ToLowerInvariant()).Distinct().Count() == measurements.Count)
            .WithMessage("Measurement keys must be unique.");

        RuleFor(x => x.proofDocuments)
            .Must(documents => documents is null ||
                               documents.All(document => document.UploadId != Guid.Empty) &&
                               documents.Select(document => document.UploadId).Distinct().Count() == documents.Count &&
                               documents.Count <= 5)
            .WithMessage("Proof document upload ids must be unique, non-empty, and limited to 5 documents.");
    }
}
