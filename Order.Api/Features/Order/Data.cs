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
        public string Client { get; set; }
        public string[] Items { get; set; }
    }

    internal sealed record ProductEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }
    }
}
