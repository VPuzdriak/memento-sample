using EShop.Products.Domain;

using Memento.EventStore;

namespace EShop.Products.Api.Endpoints;

internal static class ChangePriceEndpoint
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapPut("/products/{productId:guid}/price", async (Guid productId, ChangePriceRequest request, IEventStore eventStore, CancellationToken cancellationToken) =>
            {
                var product = await eventStore.AggregateAsync<Product>(productId, cancellationToken);
                if (product is null)
                {
                    return Results.NotFound();
                }

                product.ChangePrice(request.Price);
                await eventStore.SaveAsync(product, cancellationToken);
                return Results.Ok();
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("ChangePrice")
            .WithTags("Products");

        return builder;
    }
}

internal record ChangePriceRequest(decimal Price);