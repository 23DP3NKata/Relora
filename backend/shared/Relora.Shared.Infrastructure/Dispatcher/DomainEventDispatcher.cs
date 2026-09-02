using Relora.Shared.Infrastructure.Interfaces;
using Relora.Shared.Domain.Abstractions;
using MediatR;

namespace Relora.Shared.Infrastructure.Dispatcher;

/// <summary>
/// Represents the domain event dispatcher class.
/// </summary>
public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventDispatcher"/> class.
    /// </summary>
    /// <param name="mediator">Mediator.</param>
    public DomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Performs the dispatch async operation.
    /// </summary>
    /// <param name="events">Events.</param>
    /// <param name="ct">Ct.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct)
    {
        foreach (var domainEvent in events)
        {
            await _mediator.Publish(domainEvent, ct);
        }
    }
}
