using FastEndpoints;
using Common.Data;
using Confluent.Kafka;
using Inventory.Api.Features.Product;
using Inventory.Api.Features.Product.ControlStockHistory;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddMongoDbContext(new Dictionary<Type, string>
{
    { typeof(ProductEntity), Constants.ProductsCollectionName }
});

builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection("ConsumerConfig"));
builder.Services.AddHostedService<OrderRegisteredConsumer>(p => 
    new OrderRegisteredConsumer(p.GetRequiredService<IOptions<ConsumerConfig>>(), p.GetRequiredService<IDbContext>()));

builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseFastEndpoints();

app.Run();
