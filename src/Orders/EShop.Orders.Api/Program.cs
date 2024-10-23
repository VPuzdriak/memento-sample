using EShop.Orders.Api.Endpoints;
using EShop.Orders.Api.Store;
using EShop.Orders.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IEventStore, InMemoryEventStore>();
builder.Services.AddSingleton<ISnapshotStore, InMemorySnapshotStore>();
builder.Services.AddSingleton<ICheckpointsStore, InMemoryCheckpointsStore>();

builder.Services.AddHostedService<InMemorySnapshotWorker<Order>>();

var app = builder.Build();

CreateOrderEndpoint.Map(app);
AddOrderItemEndpoint.Map(app);
GetOrderEndpoint.Map(app);

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Hello World!");

app.Run();