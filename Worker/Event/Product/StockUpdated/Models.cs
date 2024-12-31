namespace Worker.Event.Product.StockUpdated;

internal sealed class Message
{
    public string ProductID { get; set; }
    public int OperationType { get; set; }
}

internal sealed class OrderResponse
{
    public string Id { get; set; }
    public string[] Items { get; set; }
}