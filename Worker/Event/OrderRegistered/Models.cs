namespace Worker.Event.OrderRegistered;

internal sealed class UpdateProductStockRequest
{
    public int OperationType { get; set; }
    public int Quantity { get; set; }
}

internal sealed class OrderRegisteredEvent
{
    public string OrderID { get; set; }
    public string[] Items { get; set; }
}

internal sealed class ProductOutOfStockEvent
{
    public string SourceOrderID { get; set; }
    public string ProductID { get; set; }
}

internal sealed class OrderSeparatedEvent
{
    public string OrderID { get; set; }
}