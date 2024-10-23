using EShop.Orders.Api.Endpoints;
using EShop.Orders.Api.EventHandlers;
using EShop.Orders.Domain;

using Memento.EventStore.InMemory;
using Memento.EventStore.Postgres;
using Memento.EventStreaming.InMemory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInMemoryEventStore();
builder.Services.AddPostgresEventStore(builder.Configuration.GetConnectionString("Orders")!);

builder.Services.AddSnapshots<Order>("orders");
builder.Services.AddReadModels<OrderSpecs, Order>("orderSpecs");

builder.Services.AddInMemoryEventStreaming();
builder.Services.AddDomainEventHandler<OrderCreated, OrderCreatedEventHandler>();

var app = builder.Build();

CreateOrderEndpoint.Map(app);
AddOrderItemEndpoint.Map(app);
GetOrderEndpoint.Map(app);
GetOrderSpecsEndpoint.Map(app);

app.UseSwagger();
app.UseSwaggerUI();

app.Run();