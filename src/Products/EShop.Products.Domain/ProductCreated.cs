using Memento.Aggregate;

namespace EShop.Products.Domain;

public sealed record ProductCreated(Guid ProductId, string Name, decimal Price, int Quantity) : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);