using Worker;
using Worker.Event.Order.Registered;
using Worker.Event.Order.Separated;
using Worker.Event.Product.OutOfStock;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApisConfig>(builder.Configuration.GetSection("Apis"));
builder.Services.AddOrderRegisteredEvent(builder.Configuration);
builder.Services.AddOrderSeparatedEvent(builder.Configuration);
builder.Services.AddProductOutOfStockEvent(builder.Configuration);

var app = builder.Build();

app.Run();