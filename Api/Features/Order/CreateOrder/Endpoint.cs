using Orders.Api.Data;
using FastEndpoints;

namespace Orders.Api.Features.Order.CreateOrder
{
    internal sealed class Endpoint(MongoDbContext dbContext) : Endpoint<Request, Response, Mapper>
    {
        public override void Configure()
        {
            Post("/orders");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var entity = Map.ToEntity(req);

            entity.Id = Guid.NewGuid().ToString();
            entity.Created = DateTime.UtcNow;
            entity.Updated = DateTime.UtcNow;

            dbContext.Add(Constants.OrdersCollectionName, entity);

            var response = Map.FromEntity(entity);

            await SendAsync(response, 201);
        }
    }
}
