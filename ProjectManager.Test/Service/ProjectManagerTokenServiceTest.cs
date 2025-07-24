using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Xunit;

public class TokenServiceTests
{
    private readonly TokenService _tokenService;
    private readonly TokenOptions _tokenOptions = new TokenOptions
    {
        Secret = "my_super_secret_key_1234567890_!@#$%^&*",
        ExpiresDay = 7,
    };

    public TokenServiceTests()
    {
        var options = Options.Create(_tokenOptions);
        _tokenService = new TokenService(options);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidJwtToken()
    {
        var user = new User
        {
            Username = "rafael",
            Email = "rafael@example.com",
            Role = "Admin",
        };

        var tokenString = _tokenService.GenerateToken(user);

        Assert.False(string.IsNullOrEmpty(tokenString));

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(tokenString);

        Assert.Contains(jwtToken.Claims, c => c.Type == "unique_name" && c.Value == user.Username);
        Assert.Contains(jwtToken.Claims, c => c.Type == "email" && c.Value == user.Email);
        Assert.Contains(jwtToken.Claims, c => c.Type == "role" && c.Value == user.Role);

        Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
        Assert.True(jwtToken.ValidTo <= DateTime.UtcNow.AddDays(_tokenOptions.ExpiresDay + 1));
    }
}
