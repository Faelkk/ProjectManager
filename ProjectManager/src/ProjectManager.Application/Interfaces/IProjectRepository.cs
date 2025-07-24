public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(string id);
    Task<Project> CreateAsync(Project project);

    Task<Project> UpdateAsync(Project project);
    Task DeleteAsync(string id);
}
