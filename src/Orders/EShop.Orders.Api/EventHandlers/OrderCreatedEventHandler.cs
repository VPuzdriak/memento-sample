using EShop.Orders.Domain;

using Memento.EventStreaming;

namespace EShop.Orders.Api.EventHandlers;

internal sealed class OrderCreatedEventHandler : IDomainEventHandler<OrderCreated>
{
    public Task HandleAsync(OrderCreated @event, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}