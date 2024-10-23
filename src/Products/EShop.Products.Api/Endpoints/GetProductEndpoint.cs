using EShop.Products.Domain;

using Memento.EventStore;

namespace EShop.Products.Api.Endpoints;

internal static class GetProductEndpoint
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet("/products/{productId:guid}", async (Guid productId, ISnapshotStore snapshotStore, CancellationToken cancellationToken) =>
            {
                var product = await snapshotStore.LoadAsync<Product>(productId, cancellationToken);
                return product is null ? Results.NotFound() : Results.Ok(product);
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetProduct")
            .WithTags("Products");

        return builder;
    }
}