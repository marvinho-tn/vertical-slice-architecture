namespace Worker;

public class NotificationConfig
{
    public string StockManager { get; set; }
}

public class ApiConfig
{
    public string BaseUrl { get; set; }
}

public class ApisConfig
{
    public ApiConfig InventoryApi { get; set; }
    public ApiConfig OrderApi { get; set; }
    public ApiConfig NotificationApi { get; set; }
}