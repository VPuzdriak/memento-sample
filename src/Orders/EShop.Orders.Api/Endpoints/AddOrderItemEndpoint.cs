using EShop.Orders.Api.Store;
using EShop.Orders.Domain;

namespace EShop.Orders.Api.Endpoints;

internal static class AddOrderItemEndpoint
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/orders/{orderId}/items", async (IEventStore eventStore, Guid orderId,
            AddOrderItemRequest request, CancellationToken cancellationToken) =>
        {
            var order = await eventStore.AggregateAsync<Order>(orderId, cancellationToken);
            if (order is null)
            {
                return Results.NotFound();
            }

            order.AddItem(request.ItemId, request.UnitPrice, request.Quantity);

            await eventStore.SaveAsync(order, cancellationToken);

            return Results.Ok();
        })
        .Produces(StatusCodes.Status200OK)
        .WithName("AddOrderItem")
        .WithTags("Orders");

        return builder;
    }
}

internal record AddOrderItemRequest(Guid ItemId, decimal UnitPrice, int Quantity);