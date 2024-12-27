using Worker;
using Worker.Event.Order.Registered;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApisConfig>(builder.Configuration.GetSection("Apis"));
builder.Services.AddOrderRegisteredConsumer(builder.Configuration);

var app = builder.Build();

app.Run();