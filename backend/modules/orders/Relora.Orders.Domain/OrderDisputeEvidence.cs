namespace Relora.Orders.Domain;

public sealed class OrderDisputeEvidence
{
    private OrderDisputeEvidence()
    {
    }

    public OrderDisputeEvidence(Guid disputeId, string key, DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Evidence media key is required.");
        }

        Id = Guid.NewGuid();
        DisputeId = disputeId;
        Key = key.Trim();
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; private set; }
    public Guid DisputeId { get; private set; }
    public string Key { get; private set; } = default!;
    public DateTime CreatedAtUtc { get; private set; }
}
