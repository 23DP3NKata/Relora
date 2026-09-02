using Relora.Items.Application.Commands;

using FluentValidation;

namespace Relora.Items.Application.Validators;

public sealed class CreateLotCommandValidator : AbstractValidator<CreateLotCommand>
{
    public CreateLotCommandValidator()
    {
        RuleFor(x => x.SellerId).NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(120);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MinimumLength(100)
            .MaximumLength(2000);

        RuleFor(x => x.Brand)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(80);

        RuleFor(x => x.Color)
            .MaximumLength(40)
            .When(x => !string.IsNullOrWhiteSpace(x.Color));

        RuleFor(x => x.CategoryId)
            .NotEqual(Guid.Empty)
            .When(x => x.CategoryId.HasValue);

        RuleFor(x => x.Department)
            .IsInEnum()
            .When(x => x.Department.HasValue);

        RuleFor(x => x.PrimaryColorId)
            .NotEqual(Guid.Empty)
            .When(x => x.PrimaryColorId.HasValue);

        RuleFor(x => x.ModelName)
            .MaximumLength(120)
            .When(x => !string.IsNullOrWhiteSpace(x.ModelName));

        RuleFor(x => x.AcquisitionYear)
            .InclusiveBetween(1900, DateTime.UtcNow.Year)
            .When(x => x.AcquisitionYear.HasValue);

        RuleFor(x => x.ProductionYear)
            .InclusiveBetween(1800, DateTime.UtcNow.Year)
            .When(x => x.ProductionYear.HasValue);

        RuleFor(x => x)
            .Must(x => !x.AcquisitionYear.HasValue ||
                       !x.ProductionYear.HasValue ||
                       x.AcquisitionYear.Value >= x.ProductionYear.Value)
            .WithMessage("Acquisition year cannot be earlier than production year.");

        RuleFor(x => x.VintageNotes)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.VintageNotes));

        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.City)
            .MaximumLength(30);

        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.Currency)
            .Matches("^[A-Z]{3}$")
            .WithMessage("Currency must be a three-letter uppercase ISO code.");

        RuleFor(x => x.Category).IsInEnum();
        RuleFor(x => x.Gender).IsInEnum();
        RuleFor(x => x.Size).IsInEnum();
        RuleFor(x => x.Condition).IsInEnum();

        RuleFor(x => x.Age)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Style)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.ShippingPrice)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.ShippingCurrency)
            .Matches("^[A-Z]{3}$")
            .WithMessage("Shipping currency must be a three-letter uppercase ISO code.");

        RuleFor(x => x.ShippingOriginCountry)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.ShipsToCountries)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.ShippingHandlingDays)
            .InclusiveBetween(1, 30);

        RuleFor(x => x.PhotoKeys)
            .NotNull()
            .Must(keys => keys is not null && keys.Count >= 5)
            .WithMessage("At least 5 photos are required.");

        RuleFor(x => x.PhotoKeys)
            .Must(keys => keys is null || keys.Count <= 15)
            .WithMessage("Maximum 15 photos are allowed.");

        RuleForEach(x => x.PhotoKeys)
            .NotEmpty()
            .MaximumLength(500)
            .When(x => x.PhotoKeys is not null);

        RuleFor(x => x.PhotoKeys)
            .Must(keys => keys is null || keys.Distinct(StringComparer.Ordinal).Count() == keys.Count)
            .WithMessage("Photo keys must be unique.");

        RuleFor(x => x.CoverPhotoKey)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.CoverPhotoKey));

        RuleFor(x => x)
            .Must(x => string.IsNullOrWhiteSpace(x.CoverPhotoKey) ||
                       (x.PhotoKeys is not null && x.PhotoKeys.Contains(x.CoverPhotoKey.Trim(), StringComparer.Ordinal)))
            .WithMessage("Cover photo must be one of the uploaded photos.");

        RuleFor(x => x.Materials)
            .Must(materials => materials is null || materials.Count <= 3)
            .WithMessage("You can select up to 3 materials.");

        RuleFor(x => x.Materials)
            .Must(materials => materials is null ||
                               materials.All(material =>
                                   material.MaterialId != Guid.Empty &&
                                   (!material.Percentage.HasValue ||
                                    (material.Percentage.Value >= 0 && material.Percentage.Value <= 100))))
            .WithMessage("Materials must have valid ids and percentages between 0 and 100.");

        RuleFor(x => x.Materials)
            .Must(materials => materials is null ||
                                materials.Select(material => material.MaterialId).Distinct().Count() == materials.Count)
            .WithMessage("Material ids must be unique.");

        RuleFor(x => x.Measurements)
            .Must(measurements => measurements is null ||
                                  measurements.All(measurement =>
                                      !string.IsNullOrWhiteSpace(measurement.Key) &&
                                      measurement.Value > 0 &&
                                      (string.IsNullOrWhiteSpace(measurement.Unit) ||
                                       measurement.Unit.Length <= 12)))
            .WithMessage("Measurements must have a key, positive value, and valid unit.");

        RuleFor(x => x.Measurements)
            .Must(measurements => measurements is null ||
                                  measurements.Select(measurement => measurement.Key.Trim().ToLowerInvariant()).Distinct().Count() == measurements.Count)
            .WithMessage("Measurement keys must be unique.");

        RuleFor(x => x.ProofDocuments)
            .Must(documents => documents is null ||
                               documents.All(document => document.UploadId != Guid.Empty) &&
                               documents.Select(document => document.UploadId).Distinct().Count() == documents.Count &&
                               documents.Count <= 5)
            .WithMessage("Proof document upload ids must be unique, non-empty, and limited to 5 documents.");
    }
}
