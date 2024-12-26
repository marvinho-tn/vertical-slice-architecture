using Common.Data;
using FastEndpoints;

namespace Order.Api.Features.Order.GetById;

internal sealed class Endpoint(IDbContext dbContext) : Endpoint<Request, Response, Mapper>
{
    public override void Configure()
    {
        Get("orders/{Id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var order = dbContext.GetById<OrderEntity>(req.Id);

        if (order is not null)
        {
            var response = Map.FromEntity(order);

            await SendAsync(response);
        }

        await SendNotFoundAsync();
    }
}