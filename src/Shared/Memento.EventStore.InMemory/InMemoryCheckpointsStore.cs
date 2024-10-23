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

    public Task SaveCheckpointAsync(StreamingSpecs streamingSpecs, long checkpoint)
    {
        _checkpoints.AddOrUpdate(streamingSpecs.Name, checkpoint, (_, _) => checkpoint);
        return Task.CompletedTask;
    }

    public Task<long> GetCheckpointAsync(StreamingSpecs streamingSpecs) => 
        Task.FromResult(_checkpoints.GetValueOrDefault(streamingSpecs.Name, 0));
}