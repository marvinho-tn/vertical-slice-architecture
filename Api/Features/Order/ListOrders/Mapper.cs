﻿using FastEndpoints;

namespace Api.Features.Order.ListOrders
{
    internal sealed class Mapper : Mapper<Request, Response, OrderEntity>
    {
        public override Response FromEntity(OrderEntity e)
        {
            return new Response
            {
                Id = e.Id,
                Client = e.Client.Name,
                Items = e.Items.Select(x => x.Name).ToArray()
            };
        }
    }
}
