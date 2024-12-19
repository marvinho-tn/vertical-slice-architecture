using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Api.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);

            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public void Add<T>(string collectionName, T entity) where T : class
        {
            var collection = _database.GetCollection<T>(collectionName);

            collection.InsertOne(entity);
        }
    }

    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
