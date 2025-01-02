using Common.Data;
using FastEndpoints;

namespace Order.Api.Features.Order.Register
{
    internal sealed class Endpoint(IDbContext dbContext, ILogger<Endpoint> logger) : Endpoint<Request, Response, Mapper>
    {
        public override void Configure()
        {
            Post("/orders");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            logger.LogInformation("Received request for register order");
            
            var entity = Map.ToEntity(req);

            entity.Id = Guid.NewGuid().ToString();
            entity.Created = DateTime.UtcNow;
            entity.Updated = DateTime.UtcNow;

            dbContext.Add(entity);
            
            logger.LogInformation("Saving order {Id}", entity.Id);

            var @event = new Event
            {
                OrderID = entity.Id,
                Items = entity.Items.Select(c => c.Id).ToArray()
            };

            await PublishAsync(@event, Mode.WaitForAll, ct);
            
            var response = Map.FromEntity(entity);

            await SendAsync(response, 201, ct);
        }
    }
}
