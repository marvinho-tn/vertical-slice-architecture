﻿using FastEndpoints;

namespace Order.Api.Features.Order.CreateOrder
{
    internal sealed class Mapper : Mapper<Request, Response, OrderEntity>
    {
        public override OrderEntity ToEntity(Request r)
        {
            return new OrderEntity
            {
                Client = r.Client,
                Items = r.Items
                    .Select(c => new OrderEntity.Item { Id = c, Status = OrderEntity.OrderStatus.Registered})
                    .ToArray()
            };
        }

        public override Response FromEntity(OrderEntity e)
        {
            var status = default(OrderEntity.OrderStatus);
            
            var allAreRegistered = e.Items
                .All(c => c.Status == OrderEntity.OrderStatus.Registered);

            if (allAreRegistered)
                status = OrderEntity.OrderStatus.Registered;
            
            var allAreSeparated = e.Items
                .All(c => c.Status == OrderEntity.OrderStatus.Separated);
            
            if(allAreSeparated)
                status = OrderEntity.OrderStatus.Separated;
            
            var oneIsOutOfStock = e.Items
                .Any(c => c.Status == OrderEntity.OrderStatus.OutOfStock);
            
            if(oneIsOutOfStock)
                status = OrderEntity.OrderStatus.OutOfStock;
            
            return new Response
            {
                Id = e.Id,
                Client = e.Client,
                Items = e.Items.Select(c => c.Id).ToArray(),
                Status = (int) status
            };
        }
    }
}
