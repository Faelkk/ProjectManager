using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectManager.Infrastructure.Context;
using ProjectManager.Infrastructure.Mongo;
using ProjectManager.Infrastructure.Settings;

public class DatabaseSeed
{
    private readonly IMongoCollection<UserDocument> _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly AdminSettings _adminSettings;

    public DatabaseSeed(IDatabaseContext context, IPasswordHasher passwordHasher, IOptions<AdminSettings> adminOptions)
    {
        _users = context.Users;
        _passwordHasher = passwordHasher;
        _adminSettings = adminOptions.Value;
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
            adminUser.Password = _passwordHasher.HashPassword(user, _adminSettings.DefaultPassword);

            await _users.InsertOneAsync(adminUser);
        }
    }
}
