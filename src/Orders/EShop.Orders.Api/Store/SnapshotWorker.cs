using EShop.Orders.Domain.Shared;

namespace EShop.Orders.Api.Store;

internal sealed class SnapshotWorker<T>(IEventStore eventStore, ISnapshotStore snapshotStore) : BackgroundService where T : AggregateRoot
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(100, stoppingToken);
            await CreateSnapshotsAsync(stoppingToken);
        }
    }

    private async Task CreateSnapshotsAsync(CancellationToken cancellationToken)
    {
        var aggregates = await eventStore.LoadAllAggregatesAsync<T>(cancellationToken);

        foreach (var aggregate in aggregates)
        {
            await snapshotStore.SaveAsync(aggregate, cancellationToken);
        }
    }
}