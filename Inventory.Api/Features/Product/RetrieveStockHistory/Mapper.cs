using FastEndpoints;

namespace Inventory.Api.Features.Product.RetrieveStockHistory
{
    internal sealed class Mapper : Mapper<Request, Response, ProductEntity.ProductStockEntity>
    {
        public override Response FromEntity(ProductEntity.ProductStockEntity e)
        {
            return new Response
            {
                Quantity = e.Quantity,
                Operation = (int) e.Operation,
            };
        }
    }
}
