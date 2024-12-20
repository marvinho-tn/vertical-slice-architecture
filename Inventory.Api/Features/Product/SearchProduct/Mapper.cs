using FastEndpoints;

namespace Inventory.Api.Features.Product.SearchProduct
{
    internal sealed class Mapper : Mapper<Request, Response, ProductEntity>
    {
        public override Response FromEntity(ProductEntity e)
        {
            return new Response
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Price = e.Price,
                QuantityInStock = e.QuantityInStock
            };
        }
    }
}
