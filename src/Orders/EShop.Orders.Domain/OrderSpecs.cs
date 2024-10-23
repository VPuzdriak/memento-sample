using Memento.Aggregate;
using Memento.EventStore;

namespace EShop.Orders.Domain;

public sealed class OrderSpecs : ReadModel
{
    public List<OrderItemSpecs> Items { get; private set; }
    public decimal TotalPrice { get; private set; }
    public OrderLabel Label { get; set; }

    private OrderSpecs()
    {
        Items = [];
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
        TotalPrice = 0;
        Label = OrderLabel.Empty;
    }

    private void Fold(OrderItemAdded @event)
    {
        Items.Add(new OrderItemSpecs(@event.ItemId, @event.UnitPrice, @event.Quantity));
        TotalPrice += @event.UnitPrice * @event.Quantity;
        Label = TotalPrice > 1_000 ? OrderLabel.Expensive : OrderLabel.Regular;
    }
}

public enum OrderLabel
{
    Empty = 0,
    Regular = 1,
    Expensive = 2
}

public record OrderItemSpecs(Guid Id, decimal UnitPrice, int Quantity);