namespace Memento.EventStore;

public sealed record Stream(Guid Id, string AggregateTypeName);