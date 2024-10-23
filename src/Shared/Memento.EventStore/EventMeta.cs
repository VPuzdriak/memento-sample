using Memento.Aggregate;

namespace Memento.EventStore;

public sealed record EventMeta<T>(string AggregateTypeName, Guid AggregateId, T Event, long Position) where T : DomainEvent;