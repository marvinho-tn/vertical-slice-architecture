using FastEndpoints;
using Common.Data;
using Inventory.Api.Features.Product;
using Inventory.Api.Features.Product.ControlStockHistory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));
builder.Services.AddMongoDbContext(new Dictionary<Type, string>
{
    { typeof(ProductEntity), Constants.ProductsCollectionName }
});

builder.Services.AddProductStockUpdatedEvent(builder.Configuration);
builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseFastEndpoints();

app.Run();
