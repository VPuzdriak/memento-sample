using System.Collections.Concurrent;

using Memento.Aggregate;

namespace Memento.EventStore.InMemory;

public class InMemorySnapshotStore : ISnapshotStore
{
    private readonly ConcurrentDictionary<Guid, AggregateRoot> _snapshots = [];

    public Task SaveAsync<T>(T aggregate, CancellationToken cancellationToken) where T : AggregateRoot
    {
        _snapshots.AddOrUpdate(aggregate.Id, aggregate, (_, _) => aggregate);
        return Task.CompletedTask;
    }

    public Task<T?> LoadAsync<T>(Guid streamId, CancellationToken cancellationToken) where T : AggregateRoot =>
        _snapshots.TryGetValue(streamId, out var aggregate)
            ? Task.FromResult<T?>((T)aggregate)
            : Task.FromResult<T?>(null);
}