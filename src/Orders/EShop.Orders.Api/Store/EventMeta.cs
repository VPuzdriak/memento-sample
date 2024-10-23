using EShop.Orders.Domain.Shared;

namespace EShop.Orders.Api.Store;

public record EventMeta<T>(string AggregateTypeName, T Event) where T : DomainEvent;