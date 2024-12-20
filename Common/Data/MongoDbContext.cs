using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Common.Data
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

        public void Update<T>(string collectionName, string id, T entity) where T : class
        {
            var collection = _database.GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq("_id", id);

            collection.ReplaceOne(filter, entity);
        }

        public void Delete<T>(string collectionName, string id) where T : class
        {
            var collection = _database.GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq("_id", id);

            collection.DeleteOne(filter);
        }

        public T GetById<T>(string collectionName, string id) where T : class
        {
            var collection = _database.GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq("_id", id);

            return collection.Find(filter).FirstOrDefault();
        }

        public List<T> GetAll<T>(string collectionName, int page, int pageSize) where T : class
        {
            var collection = _database.GetCollection<T>(collectionName);

            return collection.Find(Builders<T>.Filter.Empty).Skip((page - 1) * pageSize).Limit(pageSize).ToList();
        }

        public bool Exists<T>(string collectionName, string id) where T : class
        {
            var collection = _database.GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq("_id", id);

            return collection.Find(filter).Any();
        }

        public List<T> Search<T>(string collectionName, Dictionary<string, string> fields, int page, int pageSize) where T : class
        {
            var collection = _database.GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Empty;

            foreach (var field in fields)
            {
                filter = filter & Builders<T>.Filter.Eq(field.Key, field.Value);
            }

            return collection.Find(filter).Skip((page - 1) * pageSize).Limit(pageSize).ToList();
        }
    }

    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
