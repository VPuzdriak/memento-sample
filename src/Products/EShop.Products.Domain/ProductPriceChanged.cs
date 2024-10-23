using Memento.Aggregate;

namespace EShop.Products.Domain;

public record ProductPriceChanged(Guid ProductId, decimal Price) : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);