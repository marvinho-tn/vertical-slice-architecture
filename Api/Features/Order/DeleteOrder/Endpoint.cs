using Orders.Api.Data;
using FastEndpoints;

namespace Orders.Api.Features.Order.DeleteOrder
{
    internal sealed class Endpoint(MongoDbContext dbContext) : Endpoint<Request, EmptyResponse>
    {
        public override void Configure()
        {
            Delete("orders/{Id}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            dbContext.Delete<OrderEntity>(Constants.OrdersCollectionName, req.Id);

            await SendAsync(new EmptyResponse(), 204);
        }
    }
}
