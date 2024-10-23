using EShop.Orders.Domain.Shared;

namespace EShop.Orders.Api.Store;

public record EventMeta<T>(string AggregateTypeName, Guid AggregateId, T Event, long Position) where T : DomainEvent;