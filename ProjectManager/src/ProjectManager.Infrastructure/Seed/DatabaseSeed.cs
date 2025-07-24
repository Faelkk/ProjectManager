using MongoDB.Bson;
using MongoDB.Driver;
using ProjectManager.Infrastructure.Context;
using ProjectManager.Infrastructure.Mongo;

public class DatabaseSeed
{
    private readonly IMongoCollection<UserDocument> _users;
    private readonly IPasswordHasher _passwordHasher;

    public DatabaseSeed(IDatabaseContext context, IPasswordHasher passwordHasher)
    {
        _users = context.Users;
        _passwordHasher = passwordHasher;
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

            var user = new User
            {
                Id = adminUser.Id,
                Username = adminUser.Username,
                Email = adminUser.Email,
                Role = adminUser.Role,
            };
            adminUser.Password = _passwordHasher.HashPassword(user, "admin123");

            await _users.InsertOneAsync(adminUser);
        }
    }
}
