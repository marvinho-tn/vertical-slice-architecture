namespace Inventory.Api.Features.Product.SearchProduct
{
    internal sealed class Request
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Fields { get; set; }
    }

    internal sealed class Response
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int QuantityInStock { get; set; }
    }
}