using System.Collections.Concurrent;

using EShop.Orders.Domain;
using EShop.Orders.Domain.Shared;

namespace EShop.Orders.Api.Store;

internal interface IEventStore
{
    Task SaveEventsAsync(AggregateRoot aggregate, CancellationToken cancellationToken);
    Task<IReadOnlyList<DomainEvent>> GetEventsAsync(Guid streamId, CancellationToken cancellationToken);
    Task<T?> LoadAsync<T>(Guid streamId, CancellationToken cancellationToken) where T : AggregateRoot;
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
        
        // When events saved - publish them to the message broker

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<DomainEvent>> GetEventsAsync(Guid streamId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<DomainEvent>>(_events.TryGetValue(streamId, out var stream)
            ? stream.Values.ToList()
            : []);

    public Task<T?> LoadAsync<T>(Guid streamId, CancellationToken cancellationToken) where T : AggregateRoot
    {
        if (!_events.TryGetValue(streamId, out var stream))
        {
            return Task.FromResult<T?>(null);
        }

        var aggregate = AggregateRoot.Load<T>(stream.Values.ToList());
        return Task.FromResult(aggregate);
    }
}