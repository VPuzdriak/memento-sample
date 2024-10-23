using Memento.Aggregate;

namespace Memento.EventStreaming;

public interface IDomainEventHandler<in TEvent> where TEvent : DomainEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}