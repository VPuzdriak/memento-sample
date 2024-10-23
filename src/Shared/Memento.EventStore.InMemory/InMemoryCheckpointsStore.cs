using System.Collections.Concurrent;

namespace Memento.EventStore.InMemory;

internal sealed class InMemoryCheckpointsStore : ICheckpointsStore
{
    private readonly ConcurrentDictionary<string, long> _checkpoints = [];

    public Task SaveCheckpointAsync(ProjectionSpecs projectionSpecs, long checkpoint)
    {
        _checkpoints.AddOrUpdate(projectionSpecs.Name, checkpoint, (_, _) => checkpoint);
        return Task.CompletedTask;
    }

    public Task<long> GetCheckpointAsync(ProjectionSpecs projectionSpecs) =>
        Task.FromResult(_checkpoints.GetValueOrDefault(projectionSpecs.Name, 0));
}