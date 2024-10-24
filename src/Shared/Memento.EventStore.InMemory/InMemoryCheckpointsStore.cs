using System.Collections.Concurrent;

namespace Memento.EventStore.InMemory;

internal sealed class InMemoryCheckpointsStore : ICheckpointsStore
{
    private readonly ConcurrentDictionary<string, long> _checkpoints = [];

    public Task SaveCheckpointAsync(ProjectionSpecs projectionSpecs, long position)
    {
        _checkpoints.AddOrUpdate(projectionSpecs.Name, position, (_, _) => position);
        return Task.CompletedTask;
    }

    public Task<long> GetCheckpointAsync(ProjectionSpecs projectionSpecs) =>
        Task.FromResult(_checkpoints.GetValueOrDefault(projectionSpecs.Name, 0));

    public Task SaveCheckpointAsync(StreamingSpecs streamingSpecs, long position)
    {
        _checkpoints.AddOrUpdate(streamingSpecs.Name, position, (_, _) => position);
        return Task.CompletedTask;
    }

    public Task<long> GetCheckpointAsync(StreamingSpecs streamingSpecs) => 
        Task.FromResult(_checkpoints.GetValueOrDefault(streamingSpecs.Name, 0));
}