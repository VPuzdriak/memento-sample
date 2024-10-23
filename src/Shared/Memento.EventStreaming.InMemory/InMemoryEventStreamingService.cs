using Memento.EventStore;

using Microsoft.Extensions.Hosting;

namespace Memento.EventStreaming.InMemory;

internal sealed class InMemoryEventStreamingService(StreamingSpecs streamingSpecs, ICheckpointsStore checkpointsStore, IEventStore eventStore, IEventStream eventStream) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(100, stoppingToken);
            await PublishEventsAsync(stoppingToken);
        }
    }

    private async Task PublishEventsAsync(CancellationToken cancellationToken)
    {
        var checkpoint = await checkpointsStore.GetCheckpointAsync(streamingSpecs);

        var newEvents = await eventStore.GetEventsMetaFromPositionAsync(checkpoint, cancellationToken);

        foreach (var eventMeta in newEvents)
        {
            await eventStream.PublishAsync(eventMeta.Event, cancellationToken);
            await checkpointsStore.SaveCheckpointAsync(streamingSpecs, eventMeta.Position);
        }
    }
}