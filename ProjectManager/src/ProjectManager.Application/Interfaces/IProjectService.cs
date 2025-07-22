namespace ProjectManager.Application.Interfaces;

public interface IProjectService
{
    Task<List<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(string id);
    Task<Project> CreateAsync(Project project);
}
