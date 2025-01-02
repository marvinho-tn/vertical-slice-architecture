using Common.Data;
using FastEndpoints;
using Notification.Api.Features;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(lb =>
{
    lb.AddConsole();
    lb.SetMinimumLevel(LogLevel.Information);
});

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));
builder.Services.AddMongoDbContext(new Dictionary<Type, string>
{
    { typeof(ProductEntity), Constants.ProductsCollectionName }
});
builder.Services.AddEmailService(builder.Configuration);
builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseStaticFiles();
app.UseFastEndpoints();

app.Run();