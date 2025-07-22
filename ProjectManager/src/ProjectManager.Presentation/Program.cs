using ProjectManager.Application.Interfaces;
using ProjectManager.Infrastructure.Context;
using ProjectManager.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddSingleton<IDatabaseContext, DatabaseContext>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();

builder
    .Services.AddGraphQLServer()
    .AddQueryType(d => d.Name("Query"))
    .AddType<ProjectQuery>()
    .AddMutationType(d => d.Name("Mutation"))
    .AddType<ProjectMutation>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

var seeder = app.Services.GetRequiredService<DatabaseSeed>();
await seeder.SeedAsync();

app.Run();
