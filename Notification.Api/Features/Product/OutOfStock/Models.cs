namespace Notification.Api.Features.Product.OutOfStock;

internal sealed class Request
{
    public string ProductId { get; set; }
    public string To { get; set; }
}