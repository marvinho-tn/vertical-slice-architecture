using MongoDB.Bson.Serialization.Attributes;

namespace Common.Data;

public sealed record OrderEntity
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public string Client { get; set; }
    public OrderItem[] Items { get; set; }

    public sealed class OrderItem
    {
        public string Id { get; set; }
        public OrderStatus Status { get; set; }
    }
        
    public enum OrderStatus
    {
        Registered = 1,
        Separated = 2,
        OutOfStock = 3,
    }
}

public record ProductEntity
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public int QuantityInStock { get; set; }
    public List<ProductStockEntity> ProductStockHistory { get; set; } = [];

    public record ProductStockEntity
    {
        public int Quantity { get; set; }
        public StockOperationType Operation { get; set; }

        public enum StockOperationType
        {
            Increase = 1,
            Decrease = 2,
            Adjust = 3,
        }
    }
}