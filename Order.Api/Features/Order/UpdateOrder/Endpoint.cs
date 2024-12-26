﻿using Common.Data;
using FastEndpoints;

namespace Order.Api.Features.Order.UpdateOrder
{
    internal sealed class Endpoint(IDbContext dbContext) : Endpoint<Request, Response, Mapper>
    {
        public override void Configure()
        {
            Put("/orders/{Id}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var entity = dbContext.GetById<OrderEntity>(req.Id);

            if (entity is not null)
            {
                entity.Client = req.Client;
                entity.Items = req.Items;
                entity.Updated = DateTime.UtcNow;
                entity.Status = (OrderEntity.OrderStatus) req.Status;

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
