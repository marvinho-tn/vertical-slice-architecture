using Common.Data;
using FastEndpoints;

namespace Inventory.Api.Features.Product.ControlStockHistory
{
    internal sealed class Endpoint(IDbContext dbContext) : Endpoint<Request, Response, Mapper>
    {
        public override void Configure()
        {
            Put("products/{Id}/stock-history");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var product = dbContext.GetById<ProductEntity>(req.Id);
            var operationType = (ProductEntity.ProductStockEntity.StockOperationType) req.OperationType;

            if (product is not null)
            {
                switch (operationType)
                {
                    case ProductEntity.ProductStockEntity.StockOperationType.Increase:
                        product.QuantityInStock += req.Quantity;
                        break;
                    case ProductEntity.ProductStockEntity.StockOperationType.Decrease:
                        product.QuantityInStock -= req.Quantity;
                        break;
                    case ProductEntity.ProductStockEntity.StockOperationType.Adjust:
                        product.QuantityInStock = req.Quantity;
                        break;
                }

                var stockHistory = new ProductEntity.ProductStockEntity
                {
                    Quantity = req.Quantity,
                    Operation = (ProductEntity.ProductStockEntity.StockOperationType) req.OperationType
                };

                product.ProductStockHistory.Add(stockHistory);

                dbContext.Update(product.Id, product);
                
                var @event = new Event
                {
                    ProductId = product.Id,
                };

                await PublishAsync(@event, Mode.WaitForAll, ct);

                var response = Map.FromEntity(product);

                await SendAsync(response, 200, ct);
            }
            else
            {
                await SendNotFoundAsync(ct);
            }
        }
    }
}
