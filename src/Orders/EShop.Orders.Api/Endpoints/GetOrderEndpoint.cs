using EShop.Orders.Api.Store;
using EShop.Orders.Domain;
using EShop.Orders.Domain.Shared;

namespace EShop.Orders.Api.Endpoints;

internal static class GetOrderEndpoint
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet("/orders/{orderId}",
                async (ISnapshotStore snapshotStore, Guid orderId, CancellationToken cancellationToken) =>
                {
                    var order = await snapshotStore.LoadAsync<Order>(orderId, cancellationToken);
                    return order is null ? Results.NotFound() : Results.Ok(order);
                })
            .Produces<Order>()
            .WithName("GetOrder")
            .WithTags("Orders");

        return builder;
    }
}