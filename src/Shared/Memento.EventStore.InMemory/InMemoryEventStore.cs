using System.Collections.Concurrent;

using Memento.Aggregate;

namespace Memento.EventStore.InMemory;

internal class InMemoryEventStore : IEventStore
{
    private int _latestEventPosition = 0;

    private readonly HashSet<Stream> _streams = [];
    private readonly ConcurrentDictionary<Guid, SortedList<DateTime, EventMeta<DomainEvent>>> _events = [];

    public Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken)
    {
        var aggregateTypeName = GetAggregateTypeName(aggregate);

        var events = aggregate.GetEvents().ToList();
        aggregate.ClearEvents();

        // Save events to the database
        _streams.Add(new Stream(aggregate.Id, aggregateTypeName));

        var eventsStream = _events.GetOrAdd(aggregate.Id, _ => new SortedList<DateTime, EventMeta<DomainEvent>>());
        foreach (var @event in events)
        {
            _latestEventPosition++;
            eventsStream.Add(@event.OccuredAtUtc, new EventMeta<DomainEvent>(aggregateTypeName, aggregate.Id, @event, _latestEventPosition));
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<EventMeta<DomainEvent>>> GetEventsMetaAsync(Guid streamId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<EventMeta<DomainEvent>>>(_events.TryGetValue(streamId, out var stream)
            ? stream.Values.ToList()
            : []);

    public Task<IReadOnlyList<EventMeta<DomainEvent>>> GetEventsMetaFromPositionAsync<T>(long position, CancellationToken cancellationToken) where T : AggregateRoot
    {
        var aggregateTypeName = GetAggregateTypeName<T>();
        var events = _events
            .SelectMany(eventsStream => eventsStream.Value)
            .Where(meta => meta.Value.Position > position && meta.Value.AggregateTypeName == aggregateTypeName)
            .Select(meta => meta.Value)
            .ToList();

        return Task.FromResult<IReadOnlyList<EventMeta<DomainEvent>>>(events);
    }

    public async Task<IReadOnlyList<DomainEvent>> GetEventsAsync(Guid streamId, CancellationToken cancellationToken)
    {
        var eventsMeta = await GetEventsMetaAsync(streamId, cancellationToken);
        return eventsMeta.Select(meta => meta.Event).ToList();
    }

    public Task<IReadOnlyList<Stream>> GetStreamsAsync<T>(CancellationToken cancellationToken) where T : AggregateRoot
    {
        var aggregateTypeName = GetAggregateTypeName<T>(default!);
        var streams = _streams.Where(stream => stream.AggregateTypeName == aggregateTypeName).ToList();
        return Task.FromResult<IReadOnlyList<Stream>>(streams);
    }

    public async Task<T?> AggregateAsync<T>(Guid streamId, CancellationToken cancellationToken) where T : AggregateRoot
    {
        var events = await GetEventsAsync(streamId, cancellationToken);
        return events.Count == 0 ? null : AggregateRoot.Load<T>(events);
    }

    public async Task<IReadOnlyList<T>> AggregateStreamsAsync<T>(CancellationToken cancellationToken) where T : AggregateRoot
    {
        var streams = await GetStreamsAsync<T>(cancellationToken);

        List<T> aggregates = [];

        foreach (var stream in streams)
        {
            var events = await GetEventsAsync(stream.Id, cancellationToken);

            if (events.Count == 0)
            {
                continue;
            }

            var aggregate = AggregateRoot.Load<T>(events);
            aggregates.Add(aggregate);
        }

        return aggregates;
    }

    private static string GetAggregateTypeName<T>(T? aggregateRoot = null) where T : AggregateRoot
    {
        var typeName = aggregateRoot is null ? typeof(T).FullName : aggregateRoot.GetType().FullName;
        return typeName ?? throw new InvalidOperationException("Aggregate type is not registered");
    }
}