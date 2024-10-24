using System.Text.Json;
using System.Text.Json.Serialization;

using Dapper;

using Memento.Aggregate;

namespace Memento.EventStore.Postgres;

internal sealed class PostgresEventStore(DbConnectionProvider dbConnectionProvider) : IEventStore
{
    public async Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken)
    {
        await using var connection = await dbConnectionProvider.GetConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        var aggregateType = aggregate.GetType();
        var aggregateTypeName = aggregateType.FullName;
        var aggregateAssemblyName = aggregateType.Assembly.GetName().Name;

        foreach (DomainEvent @event in aggregate.GetEvents())
        {
            var eventType = @event.GetType();
            var eventTypeName = eventType.FullName;
            var eventAssemblyName = eventType.Assembly.GetName().Name;
            var eventJson = JsonSerializer.Serialize(@event, eventType);

            await connection.ExecuteAsync(
                "insert into events (id, stream_id, aggregate_type_name, position, event, event_type_name) values (@Id, @StreamId, @AggregateTypeName, nextval('event_position'), @Event::jsonb, @EventTypeName)",
                new
                {
                    Id = @event.Id,
                    StreamId = aggregate.Id,
                    AggregateTypeName = $"{aggregateTypeName}, {aggregateAssemblyName}",
                    Event = eventJson,
                    EventTypeName = $"{eventTypeName}, {eventAssemblyName}"
                },
                transaction);
        }

        await transaction.CommitAsync(cancellationToken);
        aggregate.ClearEvents();
    }

    public Task<IReadOnlyList<EventMeta<DomainEvent>>> GetEventsMetaAsync(Guid streamId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<EventMeta<DomainEvent>>> GetEventsMetaFromPositionAsync(long position, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<EventMeta<DomainEvent>>>([]);

    public async Task<IReadOnlyList<EventMeta<DomainEvent>>> GetEventsMetaFromPositionAsync<T>(long position, CancellationToken cancellationToken) where T : AggregateRoot
    {
        var aggregateTypeName = GetAggregateTypeName<T>();
        await using var connection = await dbConnectionProvider.GetConnectionAsync();

        var postgresEventsMeta = await connection.QueryAsync<PostgresEventMeta>(
            "select id, stream_id, aggregate_type_name, position, event, event_type_name from events where aggregate_type_name = @AggregateTypeName and position > @Position",
            new { AggregateTypeName = aggregateTypeName, Position = position }
        );

        List<EventMeta<DomainEvent>> eventsMeta = [];

        foreach (var postgresEventMeta in postgresEventsMeta)
        {
            var eventType = Type.GetType(postgresEventMeta.EventTypeName, throwOnError: true, ignoreCase: true);

            if (eventType is null)
            {
                throw new InvalidOperationException($"Event type '{postgresEventMeta.EventTypeName}' not found.");
            }

            var @event = JsonSerializer.Deserialize(postgresEventMeta.Event, eventType);

            var eventMeta = new EventMeta<DomainEvent>(postgresEventMeta.AggregateTypeName, postgresEventMeta.StreamId, (DomainEvent)@event!, postgresEventMeta.Position);
            eventsMeta.Add(eventMeta);
        }

        return eventsMeta;
    }

    public async Task<IReadOnlyList<DomainEvent>> GetEventsAsync(Guid streamId, CancellationToken cancellationToken)
    {
        await using var connection = await dbConnectionProvider.GetConnectionAsync();

        var eventsMeta = await connection.QueryAsync<PostgresEventMeta>(
            "select id, stream_id, aggregate_type_name, position, event, event_type_name from events where stream_id = @StreamId",
            new { StreamId = streamId }
        );

        List<DomainEvent> events = [];

        foreach (PostgresEventMeta eventMeta in eventsMeta)
        {
            var eventType = Type.GetType(eventMeta.EventTypeName, throwOnError: true, ignoreCase: true);

            if (eventType is null)
            {
                throw new InvalidOperationException($"Event type '{eventMeta.EventTypeName}' not found.");
            }

            var @event = JsonSerializer.Deserialize(eventMeta.Event, eventType);
            events.Add((DomainEvent)@event!);
        }

        return events;
    }

    public Task<IReadOnlyList<Stream>> GetStreamsAsync<T>(CancellationToken cancellationToken) where T : AggregateRoot =>
        Task.FromResult<IReadOnlyList<Stream>>([]);

    public async Task<T?> AggregateAsync<T>(Guid streamId, CancellationToken cancellationToken) where T : AggregateRoot
    {
        var events = await GetEventsAsync(streamId, cancellationToken);
        return events.Count == 0 ? null : AggregateRoot.Load<T>(events);
    }

    public Task<IReadOnlyList<T>> AggregateStreamsAsync<T>(CancellationToken cancellationToken) where T : AggregateRoot =>
        Task.FromResult<IReadOnlyList<T>>([]);

    private static string GetAggregateTypeName<T>(T? aggregateRoot = null) where T : AggregateRoot
    {
        var type = aggregateRoot is null ? typeof(T) : aggregateRoot.GetType();
        var typeName = type.FullName;

        if (string.IsNullOrEmpty(typeName))
        {
            throw new InvalidOperationException("Aggregate type is not registered");
        }

        return $"{typeName}, {type.Assembly.GetName().Name}";
    }
}