using Worker;
using Worker.Features.Order.ItemStatusUpdated;
using Worker.Features.Order.Registered;
using Worker.Features.Product.StockUpdated;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApisConfig>(builder.Configuration.GetSection("Apis"));
builder.Services.Configure<NotificationConfig>(builder.Configuration.GetSection("Notification"));
builder.Services.AddOrderRegisteredConsumer(builder.Configuration);
builder.Services.AddProductStockUpdatedConsumer(builder.Configuration);
builder.Services.AddOrderItemStatusUpdatedConsumer(builder.Configuration);

var app = builder.Build();

app.Run();