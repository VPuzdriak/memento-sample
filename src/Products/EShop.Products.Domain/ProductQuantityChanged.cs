using Memento.Aggregate;

namespace EShop.Products.Domain;

public sealed record ProductQuantityChanged(Guid ProductId, int Quantity) : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);