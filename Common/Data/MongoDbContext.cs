using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Common.Data;

public interface IDbContext
{
    void Add<T>(T entity) where T : class;
    void Update<T>(string id, T entity) where T : class;
    void Delete<T>(string id) where T : class;
    T GetById<T>(string id) where T : class;
    List<T> GetAll<T>(int page, int pageSize) where T : class;
    bool Exists<T>(string id) where T : class;
    List<T> Search<T>(Dictionary<string, string> fields, int page, int pageSize) where T : class;
}

public class MongoDbContext : IDbContext
{
    private readonly Dictionary<Type, string> _collectionNames;
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings, Dictionary<Type, string> collectionNames)
    {
        _collectionNames = collectionNames;

        var client = new MongoClient(settings.Value.ConnectionString);

        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public void Add<T>(T entity) where T : class
    {
        var collectionName = _collectionNames[typeof(T)];
        var collection = _database.GetCollection<T>(collectionName);

        collection.InsertOne(entity);
    }

    public void Update<T>(string id, T entity) where T : class
    {
        var collectionName = _collectionNames[typeof(T)];
        var collection = _database.GetCollection<T>(collectionName);
        var filter = Builders<T>.Filter.Eq("_id", id);

        collection.ReplaceOne(filter, entity);
    }

    public void Delete<T>(string id) where T : class
    {
        var collectionName = _collectionNames[typeof(T)];
        var collection = _database.GetCollection<T>(collectionName);
        var filter = Builders<T>.Filter.Eq("_id", id);

        collection.DeleteOne(filter);
    }

    public T GetById<T>(string id) where T : class
    {
        var collectionName = _collectionNames[typeof(T)];
        var collection = _database.GetCollection<T>(collectionName);
        var filter = Builders<T>.Filter.Eq("_id", id);

        return collection.Find(filter).FirstOrDefault();
    }

    public List<T> GetAll<T>(int page, int pageSize) where T : class
    {
        var collectionName = _collectionNames[typeof(T)];
        var collection = _database.GetCollection<T>(collectionName);

        return collection.Find(Builders<T>.Filter.Empty).Skip((page - 1) * pageSize).Limit(pageSize).ToList();
    }

    public bool Exists<T>(string id) where T : class
    {
        var collectionName = _collectionNames[typeof(T)];
        var collection = _database.GetCollection<T>(collectionName);
        var filter = Builders<T>.Filter.Eq("_id", id);

        return collection.Find(filter).Any();
    }

    public List<T> Search<T>(Dictionary<string, string> fields, int page, int pageSize) where T : class
    {
        var collectionName = _collectionNames[typeof(T)];
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

public static class DbContextExtensions
{
    public static IServiceCollection AddMongoDbContext(this IServiceCollection services,
        Dictionary<Type, string> collectionNames)
    {
        services.AddTransient<IDbContext, MongoDbContext>(provider =>
            new MongoDbContext(provider.GetRequiredService<IOptions<MongoDbSettings>>(), collectionNames));

        return services;
    }
}