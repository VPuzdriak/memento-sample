using Memento.Aggregate;
using Memento.EventStore;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Memento.EventStreaming.InMemory;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryEventStreaming(this IServiceCollection services)
    {
        services.TryAddSingleton<IEventStream, InMemoryEventStream>();
        services.AddSingleton(new StreamingSpecs("InMemoryEventStreaming"));

        services.AddHostedService<InMemoryEventStreamingService>();
        services.AddHostedService<InMemoryEventListenerService>();

        return services;
    }

    public static IServiceCollection AddDomainEventHandler<TEvent, THandler>(this IServiceCollection services)
        where THandler : class, IDomainEventHandler<TEvent> where TEvent : DomainEvent
    {
        services.AddScoped<IDomainEventHandler<TEvent>, THandler>();
        return services;
    }
}