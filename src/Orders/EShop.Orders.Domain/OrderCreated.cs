using EShop.Orders.Domain.Shared;

namespace EShop.Orders.Domain;

public record OrderCreated(Guid OrderId, Guid CustomerId) : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);