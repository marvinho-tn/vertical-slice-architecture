using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Api.Features.Order.CreateOrder
{
    internal sealed class OrderEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }
        public OrderClientEntity Client { get; set; }
        public OrderItemEntity[] Items { get; set; }
    }

    internal sealed class OrderItemEntity
    {
        public string Name { get; set; }
    }

    internal sealed class OrderClientEntity
    {
        public string Name { get; set; }
    }
}
