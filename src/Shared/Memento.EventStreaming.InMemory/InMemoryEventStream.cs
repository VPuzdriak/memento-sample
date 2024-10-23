using System.Threading.Channels;

using Memento.Aggregate;

namespace Memento.EventStreaming.InMemory;

public class InMemoryEventStream : IEventStream
{
    private readonly Channel<DomainEvent> _channel = Channel.CreateUnbounded<DomainEvent>();

    public ValueTask PublishAsync(DomainEvent domainEvent, CancellationToken cancellationToken) =>
        _channel.Writer.WriteAsync(domainEvent, cancellationToken);

    public IAsyncEnumerable<DomainEvent> SubscribeAsync(CancellationToken cancellationToken) => 
        _channel.Reader.ReadAllAsync(cancellationToken);
}