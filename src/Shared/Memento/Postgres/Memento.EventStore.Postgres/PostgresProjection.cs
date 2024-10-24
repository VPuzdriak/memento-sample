namespace Memento.EventStore.Postgres;

internal sealed record PostgresProjection(Guid Id, string Body, string BodyTypeName);