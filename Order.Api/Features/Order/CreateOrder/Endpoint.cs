using System.Text.Json;
using Common.Data;
using FastEndpoints;

namespace Order.Api.Features.Order.CreateOrder
{
    internal sealed class Endpoint(IDbContext dbContext) : Endpoint<Request, Response, Mapper>
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
            entity.Status = OrderEntity.OrderStatus.Registered;

            dbContext.Add(entity);

            var @event = new OrderRegisteredEvent
            {
                OrderID = entity.Id,
                Items = entity.Items
            };

            await PublishAsync(@event, Mode.WaitForAll, ct);
            
            var response = Map.FromEntity(entity);

            await SendAsync(response, 201, ct);
        }
    }
}
