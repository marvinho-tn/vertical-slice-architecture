using FastEndpoints;

namespace Orders.Api.Features.Order.UpdateOrder
{
    internal sealed class Mapper : Mapper<Request, Response, OrderEntity>
    {
        public override Response FromEntity(OrderEntity e)
        {
            return new Response
            {
                Id = e.Id,
                Client = e.Client.Name,
                Items = e.Items.Select(x => x.Id).ToArray()
            };
        }
    }
}
