// Application/Services/ProjectService.cs
using ProjectManager.Application.Interfaces;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repository;

    public ProjectService(IProjectRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Project>> GetAllAsync() => _repository.GetAllAsync();

    public Task<Project?> GetByIdAsync(string id) => _repository.GetByIdAsync(id);

    public Task<Project> CreateAsync(Project project) => _repository.CreateAsync(project);

    public Task<Project> UpdateAsync(Project project) => _repository.UpdateAsync(project);

    public Task DeleteAsync(string id) => _repository.DeleteAsync(id);
}
