using Common.Data;
using Common.Serialization;
using Confluent.Kafka;
using FastEndpoints;
using Order.Api.Features.Order;
using Order.Api.Features.Order.Create;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddMongoDbContext(new Dictionary<Type, string>
{
    { typeof(OrderEntity), Constants.OrdersCollectionName },
    { typeof(ProductEntity), Constants.ProductsCollectionName }
});

builder.Services.AddOrderRegisteredEvent(builder.Configuration);
builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseFastEndpoints();
app.Run();