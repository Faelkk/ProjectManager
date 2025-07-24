using Xunit;

public class ProjectManagerDomainTest
{
    [Fact]
    public void Project_WithValidData_ShouldBeCreatedSuccessfully()
    {
        var project = new Project
        {
            Name = "Meu Projeto",
            Description = "Descrição",
            Skills = new List<string> { "C#", "MongoDB" },
            RepositoryUrl = "https://github.com/example/repo",
            ThumbnailUrl = "https://example.com/thumbnail.png",
            Id = "1",
            CreatedAt = DateTime.UtcNow,
        };

        Assert.NotNull(project);
        Assert.Equal("1", project.Id);
        Assert.Equal("Descrição", project.Description);
        Assert.Equal("https://github.com/example/repo", project.RepositoryUrl);
        Assert.Equal("https://example.com/thumbnail.png", project.ThumbnailUrl);
        Assert.NotEmpty(project.Skills);
        Assert.Equal("Meu Projeto", project.Name);
        Assert.Contains("C#", project.Skills);
    }

    [Fact]
    public void CreateUser_WithValidData_ShouldSetAllProperties()
    {
        var user = new User
        {
            Id = "123",
            Username = "rafael",
            Email = "rafael@example.com",
            Password = "securePassword",
            Role = "Admin",
        };
        Assert.NotNull(user);
        Assert.NotEmpty(user.Password);
        Assert.Equal("123", user.Id);
        Assert.Equal("rafael", user.Username);
        Assert.Equal("rafael@example.com", user.Email);
        Assert.Equal("securePassword", user.Password);
        Assert.Equal("Admin", user.Role);
    }
}
