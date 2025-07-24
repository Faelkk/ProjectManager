using Mongo2Go;
using MongoDB.Driver;
using ProjectManager.Infrastructure.Context;

public class UserRepositoryIntegrationTests : IDisposable
{
    private readonly MongoDbRunner _mongoRunner;
    private readonly IMongoClient _mongoClient;
    private readonly IMongoDatabase _database;
    private readonly IDatabaseContext _dbContext;
    private readonly IUserRepository _userRepository;

    public UserRepositoryIntegrationTests()
    {
        _mongoRunner = MongoDbRunner.Start();
        _mongoClient = new MongoClient(_mongoRunner.ConnectionString);
        _database = _mongoClient.GetDatabase("TestDb");

        _database.DropCollection("users");

        _dbContext = (IDatabaseContext)new DatabaseContextMock(_database);
        _userRepository = new UserRepository(_dbContext);
    }

    public void Dispose()
    {
        _mongoRunner.Dispose();
    }

    [Fact]
    public async Task CreateAsync_ShouldInsertUserAndReturnUserWithId()
    {
        var user = new User
        {
            Username = "johndoe",
            Email = "john@example.com",
            Password = "hashedpassword",
            Role = "User",
        };

        var createdUser = await _userRepository.CreateAsync(user);

        Assert.False(string.IsNullOrEmpty(createdUser.Id));
        Assert.Equal(user.Username, createdUser.Username);
        Assert.Equal(user.Email, createdUser.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenExists()
    {
        var user = new User
        {
            Username = "janedoe",
            Email = "jane@example.com",
            Password = "hashedpassword",
            Role = "Admin",
        };

        await _userRepository.CreateAsync(user);

        var foundUser = await _userRepository.GetByEmailAsync("jane@example.com");

        Assert.NotNull(foundUser);
        Assert.Equal(user.Username, foundUser?.Username);
        Assert.Equal(user.Email, foundUser?.Email);
        Assert.Equal(user.Role, foundUser?.Role);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenNotExists()
    {
        var user = await _userRepository.GetByEmailAsync("nonexistent@example.com");
        Assert.Null(user);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenExists()
    {
        var user = new User
        {
            Username = "bob",
            Email = "bob@example.com",
            Password = "hashedpassword",
            Role = "User",
        };

        var createdUser = await _userRepository.CreateAsync(user);

        var foundUser = await _userRepository.GetByIdAsync(createdUser.Id);

        Assert.NotNull(foundUser);
        Assert.Equal(createdUser.Id, foundUser?.Id);
        Assert.Equal(user.Username, foundUser?.Username);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var fakeId = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        await _userRepository.GetByIdAsync(fakeId);

        var user = await _userRepository.GetByIdAsync(fakeId);
        Assert.Null(user);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        await _database.DropCollectionAsync("users");

        var users = new[]
        {
            new User
            {
                Username = "user1",
                Email = "user1@example.com",
                Password = "pass1",
                Role = "User",
            },
            new User
            {
                Username = "user2",
                Email = "user2@example.com",
                Password = "pass2",
                Role = "Admin",
            },
        };

        foreach (var user in users)
        {
            await _userRepository.CreateAsync(user);
        }

        var allUsers = await _userRepository.GetAllAsync();

        Assert.NotNull(allUsers);
        Assert.Equal(2, allUsers.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingUser()
    {
        var user = new User
        {
            Username = "alice",
            Email = "alice@example.com",
            Password = "pass",
            Role = "User",
        };

        var createdUser = await _userRepository.CreateAsync(user);

        createdUser.Username = "alice_updated";
        createdUser.Email = "alice_updated@example.com";
        createdUser.Role = "Admin";

        var updatedUser = await _userRepository.UpdateAsync(createdUser);

        Assert.Equal(createdUser.Id, updatedUser.Id);
        Assert.Equal("alice_updated", updatedUser.Username);
        Assert.Equal("alice_updated@example.com", updatedUser.Email);
        Assert.Equal("Admin", updatedUser.Role);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenUserNotFound()
    {
        var fakeId = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        await _userRepository.GetByIdAsync(fakeId);

        var user = new User
        {
            Id = fakeId,
            Username = "noone",
            Email = "noone@example.com",
            Role = "User",
        };

        await Assert.ThrowsAsync<Exception>(() => _userRepository.UpdateAsync(user));
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveUser_WhenExists()
    {
        var user = new User
        {
            Username = "tom",
            Email = "tom@example.com",
            Password = "pass",
            Role = "User",
        };

        var createdUser = await _userRepository.CreateAsync(user);

        await _userRepository.DeleteAsync(createdUser.Id);

        var deletedUser = await _userRepository.GetByIdAsync(createdUser.Id);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowException_WhenUserNotFound()
    {
        var fakeId = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        await Assert.ThrowsAsync<Exception>(() => _userRepository.DeleteAsync(fakeId));
    }
}
