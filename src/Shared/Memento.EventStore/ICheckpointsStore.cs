namespace Memento.EventStore;

public interface ICheckpointsStore
{
    Task SaveCheckpointAsync(ProjectionSpecs projectionSpecs, long position);
    Task<long> GetCheckpointAsync(ProjectionSpecs projectionSpecs);
    Task SaveCheckpointAsync(StreamingSpecs streamingSpecs, long position);
    Task<long> GetCheckpointAsync(StreamingSpecs streamingSpecs);
}