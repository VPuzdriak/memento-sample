using Memento.Aggregate;

namespace EShop.Orders.Domain;

public sealed record OrderCreated(Guid OrderId, Guid CustomerId) : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);