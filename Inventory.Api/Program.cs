using FastEndpoints;
using Common.Data;
using Common.Serialization;
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

builder.Services.AddTransient<IProducer<string, ProductOutOfStockEvent>>(sp =>
    new ProducerBuilder<string, ProductOutOfStockEvent>(
            builder.Configuration.GetSection("Kafka").Get<ProducerConfig>())
        .SetValueSerializer(new CustomJsonSerializer<ProductOutOfStockEvent>())
        .Build());

builder.Services.AddTransient<IProducer<string, OrderSeparatedEvent>>(sp =>
    new ProducerBuilder<string, OrderSeparatedEvent>(
            builder.Configuration.GetSection("Kafka").Get<ProducerConfig>())
        .SetValueSerializer(new CustomJsonSerializer<OrderSeparatedEvent>())
        .Build());

builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection("ConsumerConfig"));
builder.Services.AddHostedService<OrderRegisteredConsumer>();

builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseFastEndpoints();

app.Run();
