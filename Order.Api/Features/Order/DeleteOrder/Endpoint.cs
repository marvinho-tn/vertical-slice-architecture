using Common.Data;
using FastEndpoints;

namespace Order.Api.Features.Order.DeleteOrder
{
    internal sealed class Endpoint(IDbContext dbContext) : Endpoint<Request, EmptyResponse>
    {
        public override void Configure()
        {
            Delete("orders/{Id}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            dbContext.Delete<OrderEntity>(req.Id);

            await SendNoContentAsync(ct);
        }
    }
}
