using Memento.Aggregate;

namespace EShop.Orders.Domain;

public record OrderCreated(Guid OrderId, Guid CustomerId) : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);