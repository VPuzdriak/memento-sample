using Memento.Aggregate;

using Microsoft.Extensions.Hosting;

namespace Memento.EventStore.InMemory;

internal sealed class InMemoryReadModelWorker<TModel, TAggregate>(ProjectionSpecs<TModel> projectionSpecs, ICheckpointsStore checkpointsStore, IEventStore eventStore, IReadModelStore readModelStore)
    : BackgroundService where TModel : ReadModel where TAggregate : AggregateRoot
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(100, stoppingToken);
            await UpdateReadModelsAsync(stoppingToken);
        }
    }

    private async Task UpdateReadModelsAsync(CancellationToken cancellationToken)
    {
        var checkpoint = await checkpointsStore.GetCheckpointAsync(projectionSpecs);
        var newEvents = await eventStore.GetEventsMetaFromPositionAsync<TAggregate>(checkpoint, cancellationToken);

        foreach (var eventsStream in newEvents.GroupBy(meta => meta.AggregateId))
        {
            var streamId = eventsStream.Key;
            var events = eventsStream.Select(meta => meta.Event).ToList();
            var newCheckpoint = eventsStream.Max(meta => meta.Position);

            var snapshot = await readModelStore.LoadAsync<TModel>(streamId, cancellationToken);
            snapshot = snapshot is null ? ReadModel.Load<TModel>(events) : ReadModel.LoadFromSnapshot(snapshot, events);

            await readModelStore.SaveAsync(snapshot, cancellationToken);
            await checkpointsStore.SaveCheckpointAsync(projectionSpecs, newCheckpoint);
        }
    }
}