using FastEndpoints;

namespace Inventory.Api.Features.Product.RegisterProduct
{
    internal sealed class Mapper : Mapper<Request, Response, ProductEntity>
    {
        public override ProductEntity ToEntity(Request r)
        {
            return new ProductEntity
            {
                Name = r.Name,
                Price = r.Price,
                QuantityInStock = r.QuantityInStock,
                Description = r.Description,
            };
        }

        public override Response FromEntity(ProductEntity e)
        {
            return new Response
            {
                Id = e.Id,
                Name = e.Name,
                Price = e.Price,
                QuantityInStock = e.QuantityInStock,
                Description = e.Description,
            };
        }
    }
}
