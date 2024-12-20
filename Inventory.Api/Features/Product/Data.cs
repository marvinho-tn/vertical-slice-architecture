using MongoDB.Bson.Serialization.Attributes;

namespace Inventory.Api.Features.Product
{
    internal sealed class ProductEntity
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int QuantityInStock { get; set; }
    }
}
