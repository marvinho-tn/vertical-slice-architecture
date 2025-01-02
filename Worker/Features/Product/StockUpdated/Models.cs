namespace Worker.Events.Product.StockUpdated;

internal sealed class Message
{
    public string ProductID { get; set; }
    public int OperationType { get; set; }
}