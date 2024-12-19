using Api.Data;
using FastEndpoints;

namespace Api.Features.Order.CreateOrder
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

            dbContext.Add("Orders", entity);

            var response = Map.FromEntity(entity);

            await SendAsync(response, 201);
        }
    }
}
