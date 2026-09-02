using Relora.Shared.Domain.Abstractions;

namespace Relora.Shared.Infrastructure.Interfaces;

/// <summary>
/// Represents the i domain event dispatcher interface.
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct);
}
