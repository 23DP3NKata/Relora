using Relora.Shared.Domain.Abstractions;

namespace Relora.Items.Domain;

public sealed class MeasurementDefinition : Entity<Guid>
{
    private MeasurementDefinition()
    {
    }

    public MeasurementDefinition(
        Guid id,
        string profile,
        string key,
        string labelKey,
        bool isRequired,
        int sortOrder,
        string unit,
        bool isActive) : base(id)
    {
        if (string.IsNullOrWhiteSpace(profile))
        {
            throw new ArgumentException("Measurement profile is required.");
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Measurement key is required.");
        }

        Profile = profile.Trim();
        Key = key.Trim();
        LabelKey = string.IsNullOrWhiteSpace(labelKey) ? key.Trim() : labelKey.Trim();
        IsRequired = isRequired;
        SortOrder = sortOrder;
        Unit = string.IsNullOrWhiteSpace(unit) ? "cm" : unit.Trim().ToLowerInvariant();
        IsActive = isActive;
    }

    public string Profile { get; private set; } = default!;
    public string Key { get; private set; } = default!;
    public string LabelKey { get; private set; } = default!;
    public bool IsRequired { get; private set; }
    public int SortOrder { get; private set; }
    public string Unit { get; private set; } = "cm";
    public bool IsActive { get; private set; }
}
