using EShop.Products.Api.Endpoints;
using EShop.Products.Domain;

using Memento.EventStore.InMemory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInMemoryEventStore();
builder.Services.AddSnapshots<Product>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

CreateProductEndpoint.Map(app);
GetProductEndpoint.Map(app);
ChangePriceEndpoint.Map(app);
ChangeQuantityEndpoint.Map(app);

app.Run();