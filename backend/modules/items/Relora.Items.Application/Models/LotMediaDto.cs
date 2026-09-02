namespace Relora.Items.Application.Models;

/// <summary>
/// Represents the lot media dto class.
/// </summary>
public sealed class LotMediaDto
{
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the key used by this type.
    /// </summary>
    public string Key { get; set; } = null!;
    /// <summary>
    /// Gets or sets the type used by this type.
    /// </summary>
    public string Type { get; set; } = null!;

    public string Url { get; set; } = null!;

    public int SortOrder { get; set; }

    public bool IsCover { get; set; }
}
