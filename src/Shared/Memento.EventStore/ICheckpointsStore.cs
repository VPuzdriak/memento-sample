namespace Memento.EventStore;

public interface ICheckpointsStore
{
    Task SaveCheckpointAsync(string snapshotName, long checkpoint);
    Task SaveCheckpointAsync<T>(long checkpoint);
    Task<long> GetCheckpointAsync(string snapshotName);
    Task<long> GetCheckpointAsync<T>();
}