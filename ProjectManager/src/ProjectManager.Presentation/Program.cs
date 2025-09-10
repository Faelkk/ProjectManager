using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ProjectManager.Application;
using ProjectManager.Application.Interfaces;
using ProjectManager.Application.Services;
using ProjectManager.Infrastructure.Context;
using ProjectManager.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddSingleton<IDatabaseContext, DatabaseContext>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();



builder.Services.AddScoped<DatabaseSeed>();

builder.Services.Configure<TokenOptions>(builder.Configuration.GetSection("JwtSettings"));

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secret = builder.Configuration["JwtSettings:Secret"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!)),
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
    options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
});

builder
    .Services.AddGraphQLServer()
    .AddAuthorization()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true)
    .AddQueryType(d => d.Name("Query"))
    .AddType<ProjectQuery>()
    .AddType<UserQuery>()
    .AddMutationType(d => d.Name("Mutation"))
    .AddType<ProjectMutation>()
    .AddType<UserMutation>()  .AddUploadType(); 

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.Use(
    async (context, next) =>
    {
        var user = context.User;
        Console.WriteLine($"User authenticated: {user.Identity?.IsAuthenticated}");
        Console.WriteLine(
            $"Claims: {string.Join(", ", user.Claims.Select(c => $"{c.Type}={c.Value}"))}"
        );
        await next();
    }
);

app.MapGet("/", () => "Hello World!");
app.MapGraphQL();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeed>();
    await seeder.SeedAsync();
}

app.Run();

namespace ProjectManager.Presentation
{
    public partial class Program { }
}
