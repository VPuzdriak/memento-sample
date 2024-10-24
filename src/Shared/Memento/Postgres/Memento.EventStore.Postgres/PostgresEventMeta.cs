namespace Memento.EventStore.Postgres;

internal sealed record PostgresEventMeta(Guid Id, Guid StreamId, string AggregateTypeName, long Position, string Event, string EventTypeName);