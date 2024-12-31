using MongoDB.Bson.Serialization.Attributes;

namespace Notification.Api.Features.Product;

internal sealed record ProductEntity
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public int QuantityInStock { get; set; }
    public List<ProductStockEntity> ProductStockHistory { get; set; } = [];

    internal sealed record ProductStockEntity
    {
        public int Quantity { get; set; }
        public StockOperationType Operation { get; set; }

        internal enum StockOperationType
        {
            Increase = 1,
            Decrease = 2,
            Adjust = 3,
        }
    }
}