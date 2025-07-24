using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

public class UserGraphQLTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UserGraphQLTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static string GenerateUniqueEmail()
    {
        return $"test_{Guid.NewGuid()}@example.com";
    }

    [Fact]
    public async Task CreateUser_And_Login_Should_Return_Token()
    {
        var email = GenerateUniqueEmail();

        var mutation =
            $@"
            mutation {{
                createUser(username: ""john"", email: ""{email}"", password: ""1234"") {{
                    token
                }}
            }}";

        var response = await SendGraphQLRequest(mutation);

        if (response.RootElement.TryGetProperty("errors", out var errors))
        {
            throw new Exception("GraphQL returned errors: " + errors.ToString());
        }

        var token = response
            .RootElement.GetProperty("data")
            .GetProperty("createUser")
            .GetProperty("token")
            .GetString();

        Assert.False(string.IsNullOrEmpty(token));
    }

    [Fact]
    public async Task GetUsers_Should_Require_Authentication()
    {
        var query = @"query { users { id username email } }";

        var response = await SendGraphQLRequestAllowErrors(query);

        Assert.True(response.RootElement.TryGetProperty("errors", out var errors));
        Assert.Contains("The current user is not authorized", errors.ToString());
    }

    [Fact]
    public async Task GetUsers_WithToken_Should_Return_Data()
    {
        var email = GenerateUniqueEmail();
        var token = await CreateUserAndLoginAsync(email);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            token
        );

        var query = @"query { users { id username email } }";
        var response = await SendGraphQLRequest(query);

        var users = response.RootElement.GetProperty("data").GetProperty("users");
        Assert.NotEmpty(users.EnumerateArray());
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
            Console.WriteLine("GraphQL error: " + errors.ToString());
            throw new Exception("GraphQL returned errors: " + errors.ToString());
        }

        response.EnsureSuccessStatusCode();

        return json;
    }

    private async Task<JsonDocument> SendGraphQLRequestAllowErrors(string query)
    {
        var payload = new { query };
        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _client.PostAsync("/graphql", content);
        var responseString = await response.Content.ReadAsStringAsync();

        return JsonDocument.Parse(responseString);
    }

    private async Task<string> CreateUserAndLoginAsync(string email)
    {
        var createUserMutation =
            $@"
            mutation {{
                createUser(username: ""authuser"", email: ""{email}"", password: ""1234"") {{
                    token
                }}
            }}";

        var response = await SendGraphQLRequest(createUserMutation);

        var token = response
            .RootElement.GetProperty("data")
            .GetProperty("createUser")
            .GetProperty("token")
            .GetString();

        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("Token was not returned in the response.");

        return token;
    }
}
