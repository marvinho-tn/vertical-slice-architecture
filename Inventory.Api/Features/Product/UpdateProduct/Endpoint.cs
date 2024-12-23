using FastEndpoints;
using Common.Data;
using static Inventory.Api.Features.Product.ProductEntity;

namespace Inventory.Api.Features.Product.UpdateProduct
{
    internal sealed class Endpoint(MongoDbContext dbContext) : Endpoint<Request, Response, Mapper>
    {
        public override void Configure()
        {
            Put("/products/{Id}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var entity = dbContext.GetById<ProductEntity>(Constants.ProductsCollectionName, req.Id);

            if (entity is not null)
            {
                entity.Price = req.Price;
                entity.Name = req.Name;
                entity.Description = req.Description;

                if (entity.QuantityInStock != req.QuantityInStock)
                {
                    entity.QuantityInStock = req.QuantityInStock;
                    entity.ProductStockHistory =
                    [
                        new ProductStockEntity
                                    {
                                        Quantity = entity.QuantityInStock,
                                        Operation = ProductStockEntity.StockOperationType.Adjust,
                                    }
                    ];
                }

                dbContext.Update(Constants.ProductsCollectionName, entity.Id, entity);

                var response = Map.FromEntity(entity);

                await SendAsync(response);
            }
            else
            {
                await SendNotFoundAsync();
            }
        }
    }
}
