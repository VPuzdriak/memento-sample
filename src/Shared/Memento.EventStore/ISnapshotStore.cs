using Memento.Aggregate;

namespace Memento.EventStore;

public interface ISnapshotStore
{
    Task SaveAsync<T>(T aggregate, CancellationToken cancellationToken) where T : AggregateRoot;
    Task<T?> LoadAsync<T>(Guid streamId, CancellationToken cancellationToken) where T : AggregateRoot;
}