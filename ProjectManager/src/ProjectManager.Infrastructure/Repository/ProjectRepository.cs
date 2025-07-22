using MongoDB.Driver;
using ProjectManager.Infrastructure.Context;
using ProjectManager.Infrastructure.Models;

public class ProjectRepository : IProjectRepository
{
    private readonly IDatabaseContext _context;

    public ProjectRepository(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<Project>> GetAllAsync()
    {
        var documents = await _context.Projects.Find(_ => true).ToListAsync();

        // Mapeia ProjectDocument -> Project
        return documents
            .Select(d => new Project
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                Skills = d.Skills,
                ThumbnailUrl = d.ThumbnailUrl,
                CreatedAt = d.CreatedAt,
            })
            .ToList();
    }

    public async Task<Project?> GetByIdAsync(string id)
    {
        var doc = await _context.Projects.Find(p => p.Id == id).FirstOrDefaultAsync();

        if (doc == null)
            return null;

        return new Project
        {
            Id = doc.Id,
            Name = doc.Name,
            Description = doc.Description,
            Skills = doc.Skills,
            ThumbnailUrl = doc.ThumbnailUrl,
            CreatedAt = doc.CreatedAt,
        };
    }

    public async Task<Project> CreateAsync(Project project)
    {
        var doc = new ProjectDocument
        {
            Name = project.Name,
            Description = project.Description,
            Skills = project.Skills,
            ThumbnailUrl = project.ThumbnailUrl,
            CreatedAt = project.CreatedAt,
        };

        await _context.Projects.InsertOneAsync(doc);

        project.Id = doc.Id;

        return project;
    }
}
