using System.Collections.Concurrent;

using EShop.Orders.Domain;
using EShop.Orders.Domain.Shared;

namespace EShop.Orders.Api.Store;

internal interface IEventStore
{
    Task SaveEventsAsync(AggregateRoot aggregate, CancellationToken cancellationToken);
    Task<IReadOnlyList<DomainEvent>> GetEventsAsync(Guid streamId, CancellationToken cancellationToken);
}

internal class InMemoryEventStore : IEventStore
{
    private readonly ConcurrentDictionary<Guid, SortedList<DateTime, DomainEvent>> _events = [];

    public Task SaveEventsAsync(AggregateRoot aggregate, CancellationToken cancellationToken)
    {
        var events = aggregate.GetEvents().ToList();
        aggregate.ClearEvents();

        // Save events to the database
        var stream = _events.GetOrAdd(aggregate.Id, _ => new SortedList<DateTime, DomainEvent>());

        foreach (var @event in events)
        {
            stream.Add(@event.OccuredAtUtc, @event);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<DomainEvent>> GetEventsAsync(Guid streamId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<DomainEvent>>(_events.TryGetValue(streamId, out var stream)
            ? stream.Values.ToList()
            : []);
}