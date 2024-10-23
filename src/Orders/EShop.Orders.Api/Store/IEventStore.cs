using System.Collections.Concurrent;

using EShop.Orders.Domain.Shared;

namespace EShop.Orders.Api.Store;

internal interface IEventStore
{
    Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken);
    Task<IReadOnlyList<DomainEvent>> GetEventsAsync(Guid streamId, CancellationToken cancellationToken);
    Task<T?> AggregateAsync<T>(Guid streamId, CancellationToken cancellationToken) where T : AggregateRoot;
}

internal class InMemoryEventStore(ISnapshotStore snapshotStore) : IEventStore
{
    private readonly ConcurrentDictionary<Guid, SortedList<DateTime, DomainEvent>> _streams = [];

    public async Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken)
    {
        var events = aggregate.GetEvents().ToList();
        aggregate.ClearEvents();

        // Save events to the database
        var stream = _streams.GetOrAdd(aggregate.Id, _ => new SortedList<DateTime, DomainEvent>());

        foreach (var @event in events)
        {
            stream.Add(@event.OccuredAtUtc, @event);
        }

        await snapshotStore.SaveAsync(aggregate, cancellationToken);
    }

    public Task<IReadOnlyList<DomainEvent>> GetEventsAsync(Guid streamId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<DomainEvent>>(_streams.TryGetValue(streamId, out var stream)
            ? stream.Values.ToList()
            : []);

    public Task<T?> AggregateAsync<T>(Guid streamId, CancellationToken cancellationToken) where T : AggregateRoot
    {
        if (!_streams.TryGetValue(streamId, out var stream))
        {
            return Task.FromResult<T?>(null);
        }

        var events = stream.Values.ToList();
        if (events.Count == 0)
        {
            return Task.FromResult<T?>(null);
        }

        var aggregate = AggregateRoot.Load<T>(stream.Values.ToList());
        return Task.FromResult<T?>(aggregate);
    }
}