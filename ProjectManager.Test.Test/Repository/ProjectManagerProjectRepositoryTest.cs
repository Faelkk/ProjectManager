using Mongo2Go;
using MongoDB.Driver;
using ProjectManager.Infrastructure.Context;
using ProjectManager.Infrastructure.Mongo;
using Xunit;

namespace ProjectManager.Test.Repositories;

public class ProjectRepositoryTests : IDisposable
{
    private readonly MongoDbRunner _runner;
    private readonly IMongoDatabase _database;
    private readonly IProjectRepository _repository;

    public ProjectRepositoryTests()
    {
        _runner = MongoDbRunner.Start();
        var client = new MongoClient(_runner.ConnectionString);
        _database = client.GetDatabase("InMemoryTestDb");

        var context = new DatabaseContextMock(_database);
        _repository = new ProjectRepository(context);
    }

    [Fact]
    public async Task CreateAsync_ShouldInsertProject()
    {
        var project = new Project
        {
            Name = "Test Project",
            Description = "A test project",
            RepositoryUrl = "https://github.com/test/project",
            Skills = new List<string> { "C#", ".NET" },
            ThumbnailUrl = "http://image.jpg",
            CreatedAt = DateTime.UtcNow,
        };

        var result = await _repository.CreateAsync(project);

        Assert.NotNull(result.Id);
        Assert.Equal(project.Name, result.Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnInsertedProjects()
    {
        var project1 = await _repository.CreateAsync(
            new Project { Name = "Project A", CreatedAt = DateTime.UtcNow }
        );
        var project2 = await _repository.CreateAsync(
            new Project { Name = "Project B", CreatedAt = DateTime.UtcNow }
        );

        var result = await _repository.GetAllAsync();

        Assert.Contains(result, p => p.Id == project1.Id);
        Assert.Contains(result, p => p.Id == project2.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectProject()
    {
        var created = await _repository.CreateAsync(
            new Project { Name = "Find Me", CreatedAt = DateTime.UtcNow }
        );

        var result = await _repository.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal("Find Me", result!.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyProject()
    {
        var created = await _repository.CreateAsync(
            new Project
            {
                Name = "Original",
                Description = "Before",
                CreatedAt = DateTime.UtcNow,
            }
        );

        created.Name = "Updated";
        created.Description = "After";

        var updated = await _repository.UpdateAsync(created);

        Assert.Equal("Updated", updated.Name);
        Assert.Equal("After", updated.Description);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveProject()
    {
        var created = await _repository.CreateAsync(
            new Project { Name = "To Delete", CreatedAt = DateTime.UtcNow }
        );

        await _repository.DeleteAsync(created.Id);

        var result = await _repository.GetByIdAsync(created.Id);

        Assert.Null(result);
    }

    public void Dispose()
    {
        _runner.Dispose();
    }
}
