using EShop.Orders.Api.Endpoints;
using EShop.Orders.Api.Store;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IEventStore, InMemoryEventStore>();

var app = builder.Build();

CreateOrderEndpoint.Map(app);
GetOrderEndpoint.Map(app);

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Hello World!");

app.Run();