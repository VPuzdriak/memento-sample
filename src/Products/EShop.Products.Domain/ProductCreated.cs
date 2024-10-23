using Memento.Aggregate;

namespace EShop.Products.Domain;

public record ProductCreated(Guid ProductId, string Name, decimal Price, int Quantity) : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);