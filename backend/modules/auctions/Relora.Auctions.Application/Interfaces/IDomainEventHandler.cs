using Relora.Shared.Domain.Abstractions;

namespace Relora.Auctions.Application.Interfaces;
/// <summary>
/// Represents the i domain event handler interface.
/// </summary>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task Handle(TEvent domainEvent, CancellationToken ct);
}
