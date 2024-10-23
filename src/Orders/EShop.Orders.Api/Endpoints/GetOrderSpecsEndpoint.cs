using EShop.Orders.Domain;

using Memento.EventStore;

namespace EShop.Orders.Api.Endpoints;

internal static class GetOrderSpecsEndpoint
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet("/orders/{orderId}/specs", async (Guid orderId, IReadModelStore readModelStore, CancellationToken cancellationToken) =>
            {
                var orderSpecs = await readModelStore.LoadAsync<OrderSpecs>(orderId, cancellationToken);

                if (orderSpecs is null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(orderSpecs);
            })
            .WithName("GetOrderSpecs")
            .WithTags("Orders");

        return builder;
    }
}