using FastEndpoints;

namespace Orders.Api.Features.Order.CreateOrder
{
    internal sealed class Mapper : Mapper<Request, Response, OrderEntity>
    {
        public override OrderEntity ToEntity(Request r)
        {
            return new OrderEntity
            {
                Client = r.Client,
                Items = r.Items
            };
        }

        public override Response FromEntity(OrderEntity e)
        {
            return new Response
            {
                Id = e.Id,
                Client = e.Client,
                Items = e.Items
            };
        }
    }
}
