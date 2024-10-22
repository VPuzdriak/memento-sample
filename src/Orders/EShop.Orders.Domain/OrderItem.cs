namespace EShop.Orders.Domain;

public record OrderItem(Guid ItemId, decimal UnitPrice, int Quantity);