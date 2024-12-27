using Common.Data;
using FastEndpoints;
using Order.Api.Features.Order.UpdateOrder;

namespace Order.Api.Features.Order.UpdateOrderStatus
{
    internal sealed class Endpoint(IDbContext dbContext) : Endpoint<Request, Response, Mapper>
    {
        public override void Configure()
        {
            Put("/orders/{Id}/status");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
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