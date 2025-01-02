using Common.Data;
using FastEndpoints;

namespace Inventory.Api.Features.Product.ControlStockHistory
{
    internal sealed class Mapper : Mapper<Request, Response, ProductEntity>
    {
        public override Response FromEntity(ProductEntity e)
        {
            return new Response
            {
                Id = e.Id,
                Name = e.Name,
                QuantityInStock = e.QuantityInStock
            };
        }
    }
}
