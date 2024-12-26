namespace Worker;

public class ApiConfig
{
    public string BaseUrl { get; set; }
}

public class ApisConfig
{
    public ApiConfig InventoryApi { get; set; }
    public ApiConfig OrderApi { get; set; }
}