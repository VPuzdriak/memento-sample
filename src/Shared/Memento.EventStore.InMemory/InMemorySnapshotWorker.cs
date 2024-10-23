using Memento.Aggregate;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Memento.EventStore.InMemory;

internal sealed class InMemorySnapshotWorker<T>(ProjectionSpecs<T> projectionSpecs, IEventStore eventStore, ISnapshotStore snapshotStore, ICheckpointsStore checkpointsStore)
    : BackgroundService where T : AggregateRoot
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(100, stoppingToken);
            await UpdateSnapshotsAsync(stoppingToken);
        }
    }

    private async Task UpdateSnapshotsAsync(CancellationToken cancellationToken)
    {
        var checkpoint = await checkpointsStore.GetCheckpointAsync(projectionSpecs);
        var newEvents = await eventStore.GetEventsMetaFromPositionAsync<T>(checkpoint, cancellationToken);

        foreach (var eventsStream in newEvents.GroupBy(meta => meta.AggregateId))
        {
            var streamId = eventsStream.Key;
            var events = eventsStream.Select(meta => meta.Event).ToList();
            var newCheckpoint = eventsStream.Max(meta => meta.Position);

            var snapshot = await snapshotStore.LoadAsync<T>(streamId, cancellationToken);
            snapshot = snapshot is null ? AggregateRoot.Load<T>(events) : AggregateRoot.LoadFromSnapshot(snapshot, events);

            await snapshotStore.SaveAsync(snapshot, cancellationToken);
            await checkpointsStore.SaveCheckpointAsync(projectionSpecs, newCheckpoint);
        }
    }
}