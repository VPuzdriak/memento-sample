using Memento.Aggregate;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

    public static IServiceCollection AddSnapshots<T>(this IServiceCollection services, string name, string? version = null) where T : AggregateRoot
    {
        services.AddSingleton(sp =>
        {
            var projectionSpecs = new ProjectionSpecs<T>(name, version);
            var projectionRegistry = sp.GetRequiredService<ProjectionRegistry>();
            projectionRegistry.AddProjectionSpecs(typeof(T), projectionSpecs);
            return projectionSpecs;
        });
        
        services.AddHostedService<InMemorySnapshotWorker<T>>();

        return services;
    }

    public static IServiceCollection AddReadModels<TModel, TAggregate>(this IServiceCollection services, string name, string? version = null) where TModel : ReadModel where TAggregate : AggregateRoot
    {
        services.AddSingleton(new ProjectionSpecs<TModel>(name, version));
        services.AddHostedService<InMemoryReadModelWorker<TModel, TAggregate>>();
        return services;
    }
}