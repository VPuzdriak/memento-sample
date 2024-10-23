using EShop.Products.Domain;

using Memento.EventStore;

namespace EShop.Products.Api.Endpoints;

internal static class ChangeQuantityEndpoint
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapPut("/products/{productId:guid}/quantity", async (Guid productId, ChangeQuantityRequest request, IEventStore eventStore, CancellationToken cancellationToken) =>
            {
                var product = await eventStore.AggregateAsync<Product>(productId, cancellationToken);
                if (product is null)
                {
                    return Results.NotFound();
                }

                product.ChangeQuantity(request.Quantity);
                await eventStore.SaveAsync(product, cancellationToken);
                return Results.Ok();
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("ChangeQuantity")
            .WithTags("Products");

        return builder;
    }
}

internal record ChangeQuantityRequest(int Quantity);