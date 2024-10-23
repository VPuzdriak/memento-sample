using System.Collections.Concurrent;

namespace Memento.EventStore.InMemory;

internal sealed class InMemoryCheckpointsStore : ICheckpointsStore
{
    private readonly ConcurrentDictionary<string, long> _checkpoints = [];

    public Task SaveCheckpointAsync(string snapshotName, long checkpoint)
    {
        _checkpoints.AddOrUpdate(snapshotName, checkpoint, (_, _) => checkpoint);
        return Task.CompletedTask;
    }

    public Task SaveCheckpointAsync<T>(long checkpoint)
    {
        var readModelName = GetDefaultReadModelName<T>();
        return SaveCheckpointAsync(readModelName, checkpoint);
    }

    public Task<long> GetCheckpointAsync(string snapshotName)
    {
        return Task.FromResult(_checkpoints.GetValueOrDefault(snapshotName, 0));
    }

    public Task<long> GetCheckpointAsync<T>()
    {
        var readModelName = GetDefaultReadModelName<T>();
        return GetCheckpointAsync(readModelName);
    }

    private static string GetDefaultReadModelName<T>()
    {
        var readModelName = typeof(T).FullName;

        if (string.IsNullOrEmpty(readModelName))
        {
            throw new InvalidOperationException("Read model is not registered");
        }

        return readModelName;
    }
}