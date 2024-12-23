using FastEndpoints;
using Common.Data;
using static Inventory.Api.Features.Product.ProductEntity;

namespace Inventory.Api.Features.Product.RegisterProduct
{
    internal sealed class Endpoint(IDbContext dbContext) : Endpoint<Request, Response, Mapper>
    {
        public override void Configure()
        {
            Post("/products");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var entity = Map.ToEntity(req);

            entity.Id = Guid.NewGuid().ToString();
            entity.ProductStockHistory =
            [
                new ProductStockEntity
                {
                    Quantity = entity.QuantityInStock,
                    Operation = ProductStockEntity.StockOperationType.Adjust,
                }
            ];

            dbContext.Add(entity);

            var response = Map.FromEntity(entity);

            await SendAsync(response, 201, ct);
        }
    }
}
