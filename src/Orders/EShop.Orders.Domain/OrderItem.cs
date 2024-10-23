namespace EShop.Orders.Domain;

public sealed record OrderItem(Guid ItemId, decimal UnitPrice, int Quantity);