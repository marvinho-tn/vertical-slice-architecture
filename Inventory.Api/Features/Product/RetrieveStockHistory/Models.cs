namespace Inventory.Api.Features.Product.RetrieveStockHistory
{
    internal sealed class Request
    {
        public string Id { get; set; }
    }

    internal sealed class Response
    {
        public int Quantity { get; set; }
        public int Operation { get; set; }
    }
}
