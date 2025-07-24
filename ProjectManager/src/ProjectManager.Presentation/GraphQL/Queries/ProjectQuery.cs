// API/GraphQL/Queries/ProjectQuery.cs
using ProjectManager.Application.Interfaces;

[ExtendObjectType("Query")]
public class ProjectQuery
{
    public Task<IEnumerable<Project>> GetProjects([Service] IProjectService service) =>
        service.GetAllAsync();

    public Task<Project?> GetProjectById(string id, [Service] IProjectService service) =>
        service.GetByIdAsync(id);
}
