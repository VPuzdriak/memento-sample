namespace Memento.EventStore;

public interface ICheckpointsStore
{
    Task SaveCheckpointAsync(ProjectionSpecs projectionSpecs, long checkpoint);
    Task<long> GetCheckpointAsync(ProjectionSpecs projectionSpecs);
}