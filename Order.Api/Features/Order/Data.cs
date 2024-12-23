using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Order.Api.Features.Order
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
        public OrderStatus Status { get; set; }

        internal enum OrderStatus
        {
            Registered = 1,
            Separated = 2,
            OutOfStock = 3,
        }
    }

    internal sealed record ProductEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }
    }
}
