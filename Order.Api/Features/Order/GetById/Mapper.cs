using FastEndpoints;

namespace Order.Api.Features.Order.GetById;

internal sealed class Mapper : Mapper<Request, Response, OrderEntity>
{
    public override Response FromEntity(OrderEntity e)
    {
        return new Response
        {
            Id = e.Id,
            Client = e.Client,
            Status = (int) e.Status,
            Items = e.Items
        };
    }
}