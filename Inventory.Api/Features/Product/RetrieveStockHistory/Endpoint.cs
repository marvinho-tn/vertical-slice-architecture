using Common.Data;
using FastEndpoints;

namespace Inventory.Api.Features.Product.RetrieveStockHistory
{
    internal sealed class Endpoint(IDbContext dbContext) : Endpoint<Request, Response[], Mapper>
    {
        public override void Configure()
        {
            Get("products/{Id}/stock-history");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var product = dbContext.GetById<ProductEntity>(req.Id);

            if (product is not null)
            {
                var response = product.ProductStockHistory.Select(Map.FromEntity).ToArray();

                await SendAsync(response, 200, ct);
            }
            else
            {
                await SendNotFoundAsync(ct);
            }
        }
    }
}
