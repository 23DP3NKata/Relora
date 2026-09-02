using Relora.Shared.Domain.Abstractions;

namespace Relora.Items.Domain;

public sealed class PendingLotProofDocumentUpload : Entity<Guid>
{
    private PendingLotProofDocumentUpload()
    {
    }

    private PendingLotProofDocumentUpload(
        Guid id,
        Guid ownerId,
        Guid documentTypeId,
        string originalFileName,
        string storageKey,
        string mimeType,
        long sizeBytes,
        DateTime createdAtUtc) : base(id)
    {
        if (ownerId == Guid.Empty)
        {
            throw new ArgumentException("Owner id is required.");
        }

        if (documentTypeId == Guid.Empty)
        {
            throw new ArgumentException("Document type id is required.");
        }

        if (string.IsNullOrWhiteSpace(storageKey))
        {
            throw new ArgumentException("Storage key is required.");
        }

        if (sizeBytes <= 0)
        {
            throw new ArgumentException("File size must be positive.");
        }

        OwnerId = ownerId;
        DocumentTypeId = documentTypeId;
        OriginalFileName = NormalizeOriginalFileName(originalFileName);
        StorageKey = storageKey.Trim();
        MimeType = string.IsNullOrWhiteSpace(mimeType)
            ? "application/octet-stream"
            : mimeType.Trim().ToLowerInvariant();
        SizeBytes = sizeBytes;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid OwnerId { get; private set; }
    public Guid DocumentTypeId { get; private set; }
    public string OriginalFileName { get; private set; } = default!;
    public string StorageKey { get; private set; } = default!;
    public string MimeType { get; private set; } = default!;
    public long SizeBytes { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static PendingLotProofDocumentUpload Create(
        Guid ownerId,
        Guid documentTypeId,
        string originalFileName,
        string storageKey,
        string mimeType,
        long sizeBytes,
        DateTime createdAtUtc)
    {
        return new PendingLotProofDocumentUpload(
            Guid.NewGuid(),
            ownerId,
            documentTypeId,
            originalFileName,
            storageKey,
            mimeType,
            sizeBytes,
            createdAtUtc);
    }

    private static string NormalizeOriginalFileName(string fileName)
    {
        var normalized = Path.GetFileName(fileName ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(normalized))
        {
            return "document";
        }

        return normalized.Length <= 240 ? normalized : normalized[..240];
    }
}
