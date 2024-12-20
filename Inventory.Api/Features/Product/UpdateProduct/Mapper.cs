using FastEndpoints;

namespace Inventory.Api.Features.Product.UpdateProduct
{
    internal sealed class Mapper : Mapper<Request, Response, ProductEntity>
    {
        public override ProductEntity ToEntity(Request r)
        {
            return new ProductEntity
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Price = r.Price,
                QuantityInStock = r.QuantityInStock
            };
        }

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
