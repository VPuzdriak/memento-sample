namespace Memento.EventStore;

public record Stream(Guid Id, string AggregateTypeName);