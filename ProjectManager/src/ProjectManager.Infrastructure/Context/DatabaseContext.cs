using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProjectManager.Infrastructure.Models;
using ProjectManager.Infrastructure.Mongo;
using ProjectManager.Infrastructure.Settings;

namespace ProjectManager.Infrastructure.Context;

public class DatabaseContext : IDatabaseContext
{
    private readonly IMongoDatabase _database;

    public DatabaseContext(IOptions<DatabaseSettings> options)
    {
        var mongoClient = new MongoClient(options.Value.ConnectionString);
        _database = mongoClient.GetDatabase(options.Value.DatabaseName);
    }

    public IMongoCollection<ProjectDocument> Projects =>
        _database.GetCollection<ProjectDocument>("projects");

    public IMongoCollection<UserDocument> Users => _database.GetCollection<UserDocument>("users");
}
