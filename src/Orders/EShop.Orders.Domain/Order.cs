using EShop.Orders.Domain.Shared;

namespace EShop.Orders.Domain;

public sealed class Order : AggregateRoot
{
    public Guid CustomerId { get; set; }

    private Order()
    {
    }

    public Order(Guid id, Guid customerId)
    {
        Raise(new OrderCreated(id, customerId));
    }

    protected override void Fold(DomainEvent @event)
    {
        switch (@event)
        {
            case OrderCreated orderCreated:
                Fold(orderCreated);
                break;
        }

        base.Fold(@event);
    }

    private void Fold(OrderCreated @event)
    {
        Id = @event.OrderId;
        CustomerId = @event.CustomerId;
    }
}