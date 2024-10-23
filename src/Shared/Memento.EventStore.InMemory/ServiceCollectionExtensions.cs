using Memento.Aggregate;

using Microsoft.Extensions.DependencyInjection;

namespace Memento.EventStore.InMemory;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryEventStore(this IServiceCollection services)
    {
        services.AddSingleton<IEventStore, InMemoryEventStore>();
        services.AddSingleton<ISnapshotStore, InMemorySnapshotStore>();
        services.AddSingleton<ICheckpointsStore, InMemoryCheckpointsStore>();

        return services;
    }

    public static IServiceCollection AddSnapshots<T>(this IServiceCollection services) where T : AggregateRoot
    {
        services.AddHostedService<InMemorySnapshotWorker<T>>();
        return services;
    }
}