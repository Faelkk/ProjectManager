// API/GraphQL/Mutations/ProjectMutation.cs
using ProjectManager.Application.Interfaces;

public class ProjectMutation
{
    public Task<Project> CreateProject(
        string name,
        string description,
        List<string> skills,
        string? thumbnailUrl,
        [Service] IProjectService service
    )
    {
        var project = new Project
        {
            Name = name,
            Description = description,
            Skills = skills,
            ThumbnailUrl = thumbnailUrl,
            CreatedAt = DateTime.UtcNow,
        };

        return service.CreateAsync(project);
    }
}
