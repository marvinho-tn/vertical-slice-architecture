using FastEndpoints;
using Common.Data;
using Inventory.Api.Features.Product;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddMongoDbContext(new Dictionary<Type, string>
{
    { typeof(ProductEntity), Constants.ProductsCollectionName }
});

builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseFastEndpoints();

app.Run();
