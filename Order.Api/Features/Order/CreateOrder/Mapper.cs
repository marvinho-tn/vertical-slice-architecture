using FastEndpoints;

namespace Orders.Api.Features.Order.CreateOrder
{
    internal sealed class Mapper : Mapper<Request, Response, OrderEntity>
    {
        public override OrderEntity ToEntity(Request r)
        {
            return new OrderEntity
            {
                Client = new OrderClientEntity
                {
                    Name = r.Client
                },
                Items = r.Items.Select(x => new OrderItemEntity
                {
                    Name = x
                }).ToArray()
            };
        }

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
