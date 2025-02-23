using Common.Data;
using FastEndpoints;

namespace Order.Api.Features.Order.GetByStatusAndProduct;

internal sealed class Endpoint(IDbContext dbContext, ILogger<Endpoint> logger) : Endpoint<Request, Response[], Mapper>
{
    public override void Configure()
    {
        Get("orders/status/{Status}/products/{Id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        logger.LogInformation("Received request to get orders by status and product {Id}", req.Id);
        
        var orders = dbContext.AsQueryable<OrderEntity>()
            .Where(x => x.Items
                .Any(i => i.Id == req.Id && i.Status == (OrderEntity.OrderStatus) req.Status))
            .ToList();

        var response = orders.Select(Map.FromEntity).ToArray();
        
        await SendAsync(response, 200, ct);
    }
}