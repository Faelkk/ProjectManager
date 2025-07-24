using MongoDB.Driver;
using ProjectManager.Infrastructure.Context;
using ProjectManager.Infrastructure.Models;
using ProjectManager.Infrastructure.Mongo;

public class DatabaseContextMock : IDatabaseContext
{
    private readonly IMongoDatabase _database;

    public DatabaseContextMock(IMongoDatabase database)
    {
        _database = database;
    }

    public IMongoCollection<UserDocument> Users => _database.GetCollection<UserDocument>("users");
    public IMongoCollection<ProjectDocument> Projects =>
        _database.GetCollection<ProjectDocument>("projects");
}
