using Relora.Shared.Domain.Abstractions;

namespace Relora.Items.Domain;

public sealed class Material : Entity<Guid>
{
    private Material()
    {
    }

    public Material(Guid id, string code, string nameKey, int sortOrder, bool isActive) : base(id)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Material code is required.");
        }

        if (string.IsNullOrWhiteSpace(nameKey))
        {
            throw new ArgumentException("Material name key is required.");
        }

        Code = code.Trim().ToLowerInvariant();
        NameKey = nameKey.Trim();
        SortOrder = sortOrder;
        IsActive = isActive;
    }

    public string Code { get; private set; } = default!;
    public string NameKey { get; private set; } = default!;
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
}
