public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(string id);
    Task<Project> CreateAsync(Project project);
}
