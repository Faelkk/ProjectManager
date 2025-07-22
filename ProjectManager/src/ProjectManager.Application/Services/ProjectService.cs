// Application/Services/ProjectService.cs
using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;
using ProjectManager.Infrastructure.Interfaces;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repository;

    public ProjectService(IProjectRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Project>> GetAllAsync() => _repository.GetAllAsync();

    public Task<Project?> GetByIdAsync(string id) => _repository.GetByIdAsync(id);

    public Task<Project> CreateAsync(Project project) => _repository.CreateAsync(project);
}
