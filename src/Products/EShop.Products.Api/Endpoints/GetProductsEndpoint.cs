using EShop.Products.Domain;

using Memento.EventStore;

namespace EShop.Products.Api.Endpoints;

internal static class GetProductsEndpoint
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/products", async (ISnapshotStore store, CancellationToken cancellationToken) =>
            {
                var products = await store.LoadAsync<Product>(cancellationToken);
                return Results.Ok(products);
            })
            .Produces<IReadOnlyList<Product>>()
            .WithName("GetProducts")
            .WithTags("Products");

        return app;
    }
}