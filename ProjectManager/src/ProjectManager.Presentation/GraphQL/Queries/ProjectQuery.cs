// API/GraphQL/Queries/ProjectQuery.cs
using ProjectManager.Application.Interfaces;

public class ProjectQuery
{
    public Task<List<Project>> GetProjects([Service] IProjectService service) =>
        service.GetAllAsync();

    public Task<Project?> GetProjectById(string id, [Service] IProjectService service) =>
        service.GetByIdAsync(id);
}
