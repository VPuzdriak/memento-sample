using Memento.Aggregate;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Memento.EventStreaming.InMemory;

internal sealed class InMemoryEventListenerService(IServiceProvider serviceProvider, IEventStream eventStream) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await foreach (DomainEvent @event in eventStream.SubscribeAsync(stoppingToken))
            {
                var eventType = @event.GetType();

                await using var scope = serviceProvider.CreateAsyncScope();
                IEnumerable<object?> handlers = scope.ServiceProvider.GetServices(typeof(IDomainEventHandler<>).MakeGenericType(eventType));

                foreach (object? handler in handlers)
                {
                    if (handler is null)
                    {
                        continue;
                    }

                    var method = handler.GetType().GetMethod(nameof(IDomainEventHandler<DomainEvent>.HandleAsync));
                    await (Task)method!.Invoke(handler, [@event, stoppingToken])!;
                }
            }
        }
    }
}