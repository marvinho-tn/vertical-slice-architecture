namespace Worker.Event.Order.Registered;

internal sealed class Request
{
    public int OperationType { get; set; }
    public int Quantity { get; set; }
}

internal sealed class Message
{
    public string OrderID { get; set; }
    public string[] Items { get; set; }
}

internal sealed class OrderSeparatedEvent
{
    public string OrderID { get; set; }
}

internal sealed class ProductOutOfStockEvent
{
    public string SourceOrderID { get; set; }
    public string ProductID { get; set; }
}