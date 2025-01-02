using Common.Data;
using FastEndpoints;

namespace Order.Api.Features.Order.UpdateItemStatus
{
    internal sealed class Endpoint(IDbContext dbContext, ILogger<Endpoint> logger) : Endpoint<Request, Response, Mapper>
    {
        public override void Configure()
        {
            Put("/orders/{Id}/status");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            logger.LogInformation("Received request to update item status for order {Id}", req.Id);
            
            var entity = dbContext.GetById<OrderEntity>(req.Id);

            if (entity is not null)
            {
                entity.Updated = DateTime.UtcNow;

                foreach (var item in entity.Items)
                {
                    if (item.Id == req.ItemId)
                        item.Status = (OrderEntity.OrderStatus)req.Status;
                }

                dbContext.Update(entity.Id, entity);
                
                logger.LogInformation("Updated item status for order {Id}", req.Id);

                await PublishAsync(new Event
                {
                    OrderId = entity.Id,
                    ItemId = req.ItemId,
                    Status = req.Status
                }, Mode.WaitForAll, ct);

                var response = Map.FromEntity(entity);

                await SendAsync(response, 200, ct);
            }
            else
            {
                await SendNotFoundAsync(ct);
            }
        }
    }
}