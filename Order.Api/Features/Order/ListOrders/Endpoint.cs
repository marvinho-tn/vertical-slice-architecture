using Common.Data;
using FastEndpoints;

namespace Order.Api.Features.Order.ListOrders
{
    internal sealed class Endpoint(IDbContext dbContext) : Endpoint<Request, IEnumerable<Response>, Mapper>
    {
        public override void Configure()
        {
            Get("/orders");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var orders = dbContext.GetAll<OrderEntity>(req.Page, req.PageSize);
            var response = orders.Select(Map.FromEntity);

            await SendAsync(response, 200, ct);
        }
    }
}
