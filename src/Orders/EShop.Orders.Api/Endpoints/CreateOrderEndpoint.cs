using EShop.Orders.Domain;

using Memento.EventStore;

namespace EShop.Orders.Api.Endpoints;

internal static class CreateOrderEndpoint
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapPost("/orders",
                async (CreateOrderRequest request, IEventStore eventStore, CancellationToken cancellationToken) =>
                {
                    var order = new Order(Guid.NewGuid(), request.CustomerId);
                    await eventStore.SaveAsync(order, cancellationToken);

                    return new CreateOrderResponse(order.Id);
                })
            .Produces<CreateOrderResponse>(StatusCodes.Status201Created)
            .WithName("CreateOrder")
            .WithTags("Orders");

        return builder;
    }
}

internal record CreateOrderResponse(Guid OrderId);

internal record CreateOrderRequest(Guid CustomerId);