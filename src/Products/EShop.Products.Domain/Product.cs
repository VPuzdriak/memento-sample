using Memento.Aggregate;

namespace EShop.Products.Domain;

public sealed class Product : AggregateRoot
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }

    private Product()
    {
        Name = string.Empty;
    }

    public Product(Guid id, string name, decimal price, int quantity)
    {
        Raise(new ProductCreated(id, name, price, quantity));
    }

    protected override void Fold(DomainEvent @event)
    {
        switch (@event)
        {
            case ProductCreated created:
                Fold(created);
                break;
            case ProductPriceChanged priceChanged:
                Fold(priceChanged);
                break;
            case ProductQuantityChanged quantityChanged:
                Fold(quantityChanged);
                break;
        }
    }

    public void ChangePrice(decimal price)
    {
        Raise(new ProductPriceChanged(Id, price));
    }

    public void ChangeQuantity(int quantity)
    {
        Raise(new ProductQuantityChanged(Id, quantity));
    }

    private void Fold(ProductCreated @event)
    {
        Id = @event.Id;
        Name = @event.Name;
        Price = @event.Price;
        Quantity = @event.Quantity;
    }

    private void Fold(ProductPriceChanged @event)
    {
        Price = @event.Price;
    }

    private void Fold(ProductQuantityChanged @event)
    {
        Quantity = @event.Quantity;
    }
}