using Common.Data;
using FastEndpoints;

namespace Notification.Api.Features.Product.OutOfStock;

internal sealed class Endpoint(IEmailService emailService, IDbContext dbContext, IWebHostEnvironment env) : Endpoint<Request>
{
    public override void Configure()
    {
        Put("products/{ProductId}/out-of-stock");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var product = dbContext.GetById<ProductEntity>(req.ProductId);
        var templatePath = Path.Combine(env.WebRootPath, Constants.EmailTemplatesPathName, Constants.TemplateEmailProductOutOfStockPath);
        var content = product.GetEmailTemplate(templatePath);

        await emailService.SendEmailAsync(req.To, $"[Alert] {product.Name} is out of stock", content);
        await SendNoContentAsync(ct);
    }
}