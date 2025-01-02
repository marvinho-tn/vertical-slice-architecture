using FastEndpoints;
using Common.Data;

namespace Inventory.Api.Features.Product.RegisterProduct
{
    internal sealed class Endpoint(IDbContext dbContext, ILogger<Endpoint> logger) : Endpoint<Request, Response, Mapper>
    {
        public override void Configure()
        {
            Post("/products");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            logger.LogInformation("Received request to register product");
            
            var entity = Map.ToEntity(req);

            entity.Id = Guid.NewGuid().ToString();
            entity.ProductStockHistory =
            [
                new ProductEntity.ProductStockEntity
                {
                    Quantity = entity.QuantityInStock,
                    Operation = ProductEntity.ProductStockEntity.StockOperationType.Adjust,
                }
            ];

            dbContext.Add(entity);
            
            logger.LogInformation("Saving product {Id}", entity.Id);

            var response = Map.FromEntity(entity);

            await SendAsync(response, 201, ct);
        }
    }
}
