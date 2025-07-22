using MongoDB.Bson;
using MongoDB.Driver;
using ProjectManager.Infrastructure.Context;
using ProjectManager.Infrastructure.Mongo;

public class DatabaseSeed
{
    private readonly IMongoCollection<UserDocument> _users;

    public DatabaseSeed(IDatabaseContext context)
    {
        _users = context.Users;
    }

    public async Task SeedAsync()
    {
        var count = await _users.CountDocumentsAsync(FilterDefinition<UserDocument>.Empty);
        if (count == 0)
        {
            var adminUser = new UserDocument
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Username = "Admin",
                Email = "admin@domain.com",
                Role = "Admin",
            };

            await _users.InsertOneAsync(adminUser);
        }
    }
}
