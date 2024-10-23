namespace EShop.Orders.Domain.Shared;

public abstract record DomainEvent(Guid Id, DateTime OccuredAtUtc);