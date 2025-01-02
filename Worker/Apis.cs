using Refit;

namespace Worker;

public interface INotificationApi
{
    [Put("/products/{id}/out-of-stock")]
    Task SendOutOfStockNotificationAsync(string id, SendOutOfStockNotificationRequest req);
}

public interface IInventoryApi
{
    [Put("/products/{id}/stock-history")]
    Task<UpdateStockHistoryResponse> UpdateStockHistoryAsync(string id, UpdateStockHistoryRequest req);
}

public interface IOrderApi
{
    [Put("/orders/{id}/status")]
    Task UpdateOrderStatusAsync(string id, UpdateOrderStatusRequest req);
    
    [Get("/orders/status/{status}/products/{productId}")]
    Task<IEnumerable<GetOrdersByStatusAndProductIdResponse>> GetOrdersByStatusAndProductIdAsync(int status, string productId);
}