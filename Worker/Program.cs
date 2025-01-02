using Confluent.Kafka;
using Worker;
using Worker.Features.Order.ItemStatusUpdated;
using Worker.Features.Order.Registered;
using Worker.Features.Product.StockUpdated;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(lb =>
{
    lb.AddConsole();
    lb.SetMinimumLevel(LogLevel.Information);
});

builder.Services.Configure<ApisConfig>(builder.Configuration.GetSection("Apis"));
builder.Services.Configure<NotificationConfig>(builder.Configuration.GetSection("Notification"));
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection("Kafka:Consumer"));
builder.Services.AddOrderRegisteredConsumer();
builder.Services.AddProductStockUpdatedConsumer();
builder.Services.AddOrderItemStatusUpdatedConsumer();

var app = builder.Build();

app.Run();