using Memento.Aggregate;

using Microsoft.Extensions.DependencyInjection;

namespace Memento.EventStore.InMemory;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryEventStore(this IServiceCollection services)
    {
        services.AddSingleton<IEventStore, InMemoryEventStore>();
        services.AddSingleton<ISnapshotStore, InMemorySnapshotStore>();
        services.AddSingleton<IReadModelStore, InMemoryReadModelStore>();
        services.AddSingleton<ICheckpointsStore, InMemoryCheckpointsStore>();

        return services;
    }

    public static IServiceCollection AddSnapshots<T>(this IServiceCollection services) where T : AggregateRoot
    {
        services.AddHostedService<InMemorySnapshotWorker<T>>();
        return services;
    }

    public static IServiceCollection AddReadModels<TModel, TAggregate>(this IServiceCollection services) where TModel : ReadModel where TAggregate : AggregateRoot
    {
        services.AddHostedService<InMemoryReadModelWorker<TModel, TAggregate>>();
        return services;
    }
}