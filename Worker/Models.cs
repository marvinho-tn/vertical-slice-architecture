namespace Worker;

public class SendOutOfStockNotificationRequest
{
    public string To { get; set; }
}

public class UpdateStockHistoryRequest
{
    public int OperationType { get; set; }
    public int Quantity { get; set; }
}

public class UpdateStockHistoryResponse { }

public class UpdateOrderStatusRequest
{
    public string ItemId { get; set; }
    public int Status { get; set; }
}

public class GetOrdersByStatusAndProductIdResponse
{
    public string Id { get; set; }
    public string[] Items { get; set; }
}