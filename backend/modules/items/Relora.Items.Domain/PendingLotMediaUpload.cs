using Relora.Shared.Domain.Abstractions;

namespace Relora.Items.Domain;

public sealed class PendingLotMediaUpload : Entity<Guid>
{
    private PendingLotMediaUpload()
    {
    }

    private PendingLotMediaUpload(Guid id, Guid ownerId, string key, DateTime createdAtUtc)
        : base(id)
    {
        if (ownerId == Guid.Empty)
        {
            throw new ArgumentException("Media owner id is required.");
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Media key is required.");
        }

        var normalizedKey = key.Trim();
        if (normalizedKey.Length > 500)
        {
            throw new ArgumentException("Media key is too long.");
        }

        OwnerId = ownerId;
        Key = normalizedKey;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid OwnerId { get; private set; }
    public string Key { get; private set; } = default!;
    public DateTime CreatedAtUtc { get; private set; }

    public static PendingLotMediaUpload Create(Guid ownerId, string key, DateTime createdAtUtc)
    {
        return new PendingLotMediaUpload(Guid.NewGuid(), ownerId, key, createdAtUtc);
    }
}
