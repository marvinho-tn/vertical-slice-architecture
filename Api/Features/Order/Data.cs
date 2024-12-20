using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Orders.Api.Features.Order
{
    internal sealed record OrderEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public OrderClientEntity Client { get; set; }
        public OrderItemEntity[] Items { get; set; }
    }

    internal sealed record OrderItemEntity
    {
        public string Name { get; set; }
    }

    internal sealed record OrderClientEntity
    {
        public string Name { get; set; }
    }
}
