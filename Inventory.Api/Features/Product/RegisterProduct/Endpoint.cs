using FastEndpoints;
using Orders.Api.Data;

namespace Inventory.Api.Features.Product.RegisterProduct
{
    internal sealed class Endpoint(MongoDbContext dbContext) : Endpoint<Request, Response, Mapper>
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

            dbContext.Add("Products", entity);

            var response = Map.FromEntity(entity);

            await SendAsync(response, 201);
        }
    }
}
