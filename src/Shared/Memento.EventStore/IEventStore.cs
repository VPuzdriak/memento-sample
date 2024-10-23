using Memento.Aggregate;

namespace Memento.EventStore;

public interface IEventStore
{
    Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken);
    Task<IReadOnlyList<EventMeta<DomainEvent>>> GetEventsMetaAsync(Guid streamId, CancellationToken cancellationToken);
    Task<IReadOnlyList<EventMeta<DomainEvent>>> GetEventsMetaFromPositionAsync(long position, CancellationToken cancellationToken);
    Task<IReadOnlyList<EventMeta<DomainEvent>>> GetEventsMetaFromPositionAsync<T>(long position, CancellationToken cancellationToken) where T : AggregateRoot;
    Task<IReadOnlyList<DomainEvent>> GetEventsAsync(Guid streamId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Stream>> GetStreamsAsync<T>(CancellationToken cancellationToken) where T : AggregateRoot;
    Task<T?> AggregateAsync<T>(Guid streamId, CancellationToken cancellationToken) where T : AggregateRoot;
    Task<IReadOnlyList<T>> AggregateStreamsAsync<T>(CancellationToken cancellationToken) where T : AggregateRoot;
}