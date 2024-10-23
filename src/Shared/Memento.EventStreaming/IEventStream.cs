using Memento.Aggregate;

namespace Memento.EventStreaming;

public interface IEventStream
{
    ValueTask PublishAsync(DomainEvent domainEvent, CancellationToken cancellationToken);
    IAsyncEnumerable<DomainEvent> SubscribeAsync(CancellationToken cancellationToken);
}