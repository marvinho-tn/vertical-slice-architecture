using Api.Data;
using FastEndpoints;

namespace Api.Features.Order.ListOrders
{
    internal sealed class Endpoint(MongoDbContext dbContext) : Endpoint<Request, IEnumerable<Response>, Mapper>
    {
        public override void Configure()
        {
            Get("/orders");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request request, CancellationToken cancellationToken)
        {
            var orders = dbContext.GetAll<OrderEntity>(Constants.OrdersCollectionName, request.Page, request.PageSize);
            var response = orders.Select(Map.FromEntity);

            await SendAsync(response);
        }
    }
}
