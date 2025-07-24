using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class ProjectGraphQLTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProjectGraphQLTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        var token = CreateUserAndLoginAsync(factory.Services).GetAwaiter().GetResult();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            token
        );
    }

    private static string GenerateUniqueName() => $"project_{Guid.NewGuid()}";

    private static string GenerateUniqueDescription() => $"description_{Guid.NewGuid()}";

    private static string GenerateRepositoryUrl() => "https://github.com/example/repo";

    private static string GenerateSkillsJson() => "[\"C#\", \"GraphQL\"]";

    [Fact]
    public async Task CreateProject_Should_Return_Project()
    {
        var name = GenerateUniqueName();
        var description = GenerateUniqueDescription();
        var skillsJson = GenerateSkillsJson();
        var repositoryUrl = GenerateRepositoryUrl();

        var mutation =
            $@"
            mutation {{
                createProject(
                    name: ""{name}"", 
                    description: ""{description}"", 
                    skills: {skillsJson}, 
                    repositoryUrl: ""{repositoryUrl}""
                ) {{
                    id
                    name
                    description
                    createdAt
                }}
            }}";

        var response = await SendGraphQLRequest(mutation);

        var project = response.RootElement.GetProperty("data").GetProperty("createProject");

        Assert.Equal(name, project.GetProperty("name").GetString());
        Assert.Equal(description, project.GetProperty("description").GetString());
        Assert.NotNull(project.GetProperty("id").GetString());
    }

    [Fact]
    public async Task GetProjects_Should_Return_List()
    {
        var query = @"query { projects { id name description createdAt } }";

        var response = await SendGraphQLRequest(query);

        var projects = response.RootElement.GetProperty("data").GetProperty("projects");
        Assert.True(projects.GetArrayLength() >= 0);
    }

    [Fact]
    public async Task GetProjectById_Should_Return_Project()
    {
        var name = GenerateUniqueName();
        var description = GenerateUniqueDescription();
        var skillsJson = GenerateSkillsJson();
        var repositoryUrl = GenerateRepositoryUrl();

        var createMutation =
            $@"
            mutation {{
                createProject(
                    name: ""{name}"", 
                    description: ""{description}"", 
                    skills: {skillsJson}, 
                    repositoryUrl: ""{repositoryUrl}""
                ) {{
                    id
                    name
                    description
                    createdAt
                }}
            }}";

        var createResponse = await SendGraphQLRequest(createMutation);
        var createdProject = createResponse
            .RootElement.GetProperty("data")
            .GetProperty("createProject");
        var projectId = createdProject.GetProperty("id").GetString();

        var queryById =
            $@"
            query {{
                projectById(id: ""{projectId}"") {{
                    id
                    name
                    description
                    createdAt
                }}
            }}";

        var getResponse = await SendGraphQLRequest(queryById);
        var project = getResponse.RootElement.GetProperty("data").GetProperty("projectById");

        Assert.Equal(projectId, project.GetProperty("id").GetString());
        Assert.Equal(name, project.GetProperty("name").GetString());
        Assert.Equal(description, project.GetProperty("description").GetString());
    }

    [Fact]
    public async Task UpdateProject_Should_Modify_Project()
    {
        var name = GenerateUniqueName();
        var description = GenerateUniqueDescription();
        var skillsJson = GenerateSkillsJson();
        var repositoryUrl = GenerateRepositoryUrl();

        var createMutation =
            $@"
            mutation {{
                createProject(
                    name: ""{name}"", 
                    description: ""{description}"", 
                    skills: {skillsJson}, 
                    repositoryUrl: ""{repositoryUrl}""
                ) {{
                    id
                    name
                    description
                }}
            }}";

        var createResponse = await SendGraphQLRequest(createMutation);
        var project = createResponse.RootElement.GetProperty("data").GetProperty("createProject");
        var projectId = project.GetProperty("id").GetString();

        var newName = GenerateUniqueName();
        var newDescription = GenerateUniqueDescription();

        var updateMutation =
            $@"
            mutation {{
                updateProject(input: {{
                    id: ""{projectId}"",
                    name: ""{newName}"",
                    description: ""{newDescription}""
                }}) {{
                    id
                    name
                    description
                }}
            }}";

        var updateResponse = await SendGraphQLRequest(updateMutation);
        var updatedProject = updateResponse
            .RootElement.GetProperty("data")
            .GetProperty("updateProject");

        Assert.Equal(projectId, updatedProject.GetProperty("id").GetString());
        Assert.Equal(newName, updatedProject.GetProperty("name").GetString());
        Assert.Equal(newDescription, updatedProject.GetProperty("description").GetString());
    }

    [Fact]
    public async Task DeleteProject_Should_Return_True()
    {
        var name = GenerateUniqueName();
        var description = GenerateUniqueDescription();
        var skillsJson = GenerateSkillsJson();
        var repositoryUrl = GenerateRepositoryUrl();

        var createMutation =
            $@"
            mutation {{
                createProject(
                    name: ""{name}"", 
                    description: ""{description}"", 
                    skills: {skillsJson}, 
                    repositoryUrl: ""{repositoryUrl}""
                ) {{
                    id
                }}
            }}";

        var createResponse = await SendGraphQLRequest(createMutation);
        var projectId = createResponse
            .RootElement.GetProperty("data")
            .GetProperty("createProject")
            .GetProperty("id")
            .GetString();

        var deleteMutation =
            $@"
            mutation {{
                deleteProject(id: ""{projectId}"")
            }}";

        var deleteResponse = await SendGraphQLRequest(deleteMutation);
        var result = deleteResponse
            .RootElement.GetProperty("data")
            .GetProperty("deleteProject")
            .GetBoolean();

        Assert.True(result);
    }

    private async Task<JsonDocument> SendGraphQLRequest(string query)
    {
        var payload = new { query };
        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _client.PostAsync("/graphql", content);
        var responseString = await response.Content.ReadAsStringAsync();

        var json = JsonDocument.Parse(responseString);

        if (json.RootElement.TryGetProperty("errors", out var errors))
        {
            throw new Exception("GraphQL returned errors: " + errors.ToString());
        }

        response.EnsureSuccessStatusCode();

        return json;
    }

    public static async Task<string> CreateUserAndLoginAsync(IServiceProvider services)
    {
        var userService = services.GetRequiredService<IUserService>();

        string adminEmail = $"admin_{Guid.NewGuid()}@teste.com";

        var adminUser = new User
        {
            Username = "admin",
            Email = adminEmail,
            Role = "Admin",
            Password = "12345678",
        };

        var token = await userService.CreateAsync(adminUser);

        return token;
    }
}
