using Api.Data;
using FastEndpoints;

namespace Api.Features.Order.UpdateOrder
{
    internal sealed class Endpoint(MongoDbContext dbContext) : Endpoint<Request, Response, Mapper>
    {
        public override void Configure()
        {
            Put("/orders/{Id}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var entity = dbContext.GetById<OrderEntity>("Orders", req.Id);

            entity.Client.Name = req.Client;
            entity.Items = req.Items.Select(x => new OrderItemEntity
            {
                Name = x
            }).ToArray();
            entity.Updated = DateTime.UtcNow;

            dbContext.Update("Orders", entity.Id, entity);

            var response = Map.FromEntity(entity);

            await SendAsync(response, 200);
        }
    }
}
