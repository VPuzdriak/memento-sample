using EShop.Products.Domain;

using Memento.EventStore;

namespace EShop.Products.Api.Endpoints;

internal static class CreateProductEndpoint
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapPost("/products", async (CreateProductRequest request, IEventStore eventStore, CancellationToken cancellationToken) =>
            {
                var product = new Product(Guid.NewGuid(), request.Name, request.Price, request.Quantity);
                await eventStore.SaveAsync(product, cancellationToken);
                return new CreateProductResponse(product.Id, product.Name, product.Price, product.Quantity);
            })
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .WithName("CreateProduct")
            .WithTags("Products");

        return builder;
    }
}

internal record CreateProductRequest(string Name, decimal Price, int Quantity);

internal record CreateProductResponse(Guid ProductId, string Name, decimal Price, int Quantity);