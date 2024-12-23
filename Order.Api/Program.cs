using Common.Data;
using Common.Serialization;
using Confluent.Kafka;
using FastEndpoints;
using Order.Api.Features.Order;
using Order.Api.Features.Order.CreateOrder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddMongoDbContext(new Dictionary<Type, string>
{
    { typeof(OrderEntity), Constants.OrdersCollectionName },
    { typeof(ProductEntity), Constants.ProductsCollectionName }
});

builder.Services.AddTransient<IProducer<string, OrderRegisteredEvent>>(sp =>
    new ProducerBuilder<string, OrderRegisteredEvent>(
            builder.Configuration.GetSection("Kafka").Get<ProducerConfig>())
        .SetValueSerializer(new CustomJsonSerializer<OrderRegisteredEvent>())
        .Build());

builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseFastEndpoints();
app.Run();