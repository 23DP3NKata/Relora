namespace Relora.Shared.Domain.Abstractions;

/// <summary>
/// Represents the aggregate root class.
/// </summary>
public abstract class AggregateRoot<TId> : Entity<TId>
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

    protected AggregateRoot(TId id) : base(id)
    {
    }

    /// <summary>
    /// Adds domain event.
    /// </summary>
    /// <param name="domainEvent">Domain event.</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Performs the clear domain events operation.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
