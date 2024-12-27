using Worker;
using Worker.Event.Order.Registered;
using Worker.Event.Product.StockUpdated;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApisConfig>(builder.Configuration.GetSection("Apis"));
builder.Services.AddOrderRegisteredConsumer(builder.Configuration);
builder.Services.AddProductStockUpdatedConsumer(builder.Configuration);

var app = builder.Build();

app.Run();