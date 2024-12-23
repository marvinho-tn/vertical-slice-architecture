using Common.Data;
using FastEndpoints;

namespace Inventory.Api.Features.Product.SearchProduct
{
    internal sealed class Endpoint(IDbContext dbContext) : Endpoint<Request, List<Response>, Mapper>
    {
        public override void Configure()
        {
            Get("/products");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var products = dbContext.Search<ProductEntity>(req.FieldsAsDictionary, req.Page, req.PageSize);
            var response = products.Select(Map.FromEntity).ToList();

            await SendAsync(response, 200, ct);
        }
    }
}
