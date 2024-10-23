using Memento.Aggregate;

namespace EShop.Orders.Domain;

public sealed class Order : AggregateRoot
{
    private readonly List<OrderItem> _items = [];

    public Guid CustomerId { get; private set; }
    public IReadOnlyList<OrderItem> Items => _items;
    public DateTime CreatedAt { get; private set; }

    private Order()
    {
    }

    public Order(Guid id, Guid customerId)
    {
        Raise(new OrderCreated(id, customerId));
    }

    public void AddItem(Guid itemId, decimal unitPrice, int quantity)
    {
        Raise(new OrderItemAdded(Id, itemId, unitPrice, quantity));
    }

    protected override void Fold(DomainEvent @event)
    {
        switch (@event)
        {
            case OrderCreated orderCreated:
                Fold(orderCreated);
                break;
            case OrderItemAdded orderItemAdded:
                Fold(orderItemAdded);
                break;
        }
    }

    private void Fold(OrderCreated @event)
    {
        Id = @event.OrderId;
        CustomerId = @event.CustomerId;
        CreatedAt = @event.OccuredAtUtc;
    }

    private void Fold(OrderItemAdded @event)
    {
        // Check if item already exists
        _items.Add(new OrderItem(@event.ItemId, @event.UnitPrice, @event.Quantity));
    }
}

public record OrderItemAdded(Guid OrderId, Guid ItemId, decimal UnitPrice, int Quantity)
    : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);