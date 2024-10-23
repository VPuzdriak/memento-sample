using System.Collections.Concurrent;

using Memento.Aggregate;

namespace Memento.EventStore.InMemory;

internal sealed class InMemorySnapshotStore : ISnapshotStore
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

    public Task<IReadOnlyList<T>> LoadAsync<T>(CancellationToken cancellationToken) where T : AggregateRoot
    {
        var aggregates = _snapshots.Values.OfType<T>().ToList();
        return Task.FromResult<IReadOnlyList<T>>(aggregates);
    }
}