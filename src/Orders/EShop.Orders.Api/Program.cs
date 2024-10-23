using EShop.Orders.Api.Endpoints;
using EShop.Orders.Domain;

using Memento.EventStore.InMemory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInMemoryEventStore();
builder.Services.AddSnapshots<Order>("orders");
builder.Services.AddReadModels<OrderSpecs, Order>("orderSpecs");

var app = builder.Build();

CreateOrderEndpoint.Map(app);
AddOrderItemEndpoint.Map(app);
GetOrderEndpoint.Map(app);
GetOrderSpecsEndpoint.Map(app);

app.UseSwagger();
app.UseSwaggerUI();

app.Run();