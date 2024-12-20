using FastEndpoints;
using Common.Data;

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
            var exists = dbContext.Exists<ProductEntity>(Constants.ProductsCollectionName, req.Id);

            if (exists)
            {
                var entity = Map.ToEntity(req);

                dbContext.Update(Constants.ProductsCollectionName, req.Id, entity);

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
