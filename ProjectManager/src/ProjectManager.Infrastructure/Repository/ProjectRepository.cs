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

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        var documents = await _context.Projects.Find(_ => true).ToListAsync();

        return documents.Select(d => new Project
        {
            Id = d.Id,
            Name = d.Name,
            RepositoryUrl = d.RepositoryUrl,
            Description = d.Description,
            Skills = d.Skills,
            ThumbnailUrl = d.ThumbnailUrl,
            ThumbnailPublicId = d.ThumbnailPublicId,
            CreatedAt = d.CreatedAt,
        });
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
            RepositoryUrl = doc.RepositoryUrl,
            Description = doc.Description,
            Skills = doc.Skills,
            ThumbnailUrl = doc.ThumbnailUrl,
            ThumbnailPublicId = doc.ThumbnailPublicId,
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
            ThumbnailPublicId =    project.ThumbnailPublicId,
            RepositoryUrl = project.RepositoryUrl,
            CreatedAt = project.CreatedAt,
        };

        await _context.Projects.InsertOneAsync(doc);

        project.Id = doc.Id;

        return project;
    }

    public async Task<Project> UpdateAsync(Project project)
    {
        var filter = Builders<ProjectDocument>.Filter.Eq(p => p.Id, project.Id);

        var update = Builders<ProjectDocument>
            .Update.Set(p => p.Name, project.Name)
            .Set(p => p.Description, project.Description)
            .Set(p => p.Skills, project.Skills)
            .Set(p => p.ThumbnailUrl, project.ThumbnailUrl).Set(p => p.ThumbnailPublicId, project.ThumbnailPublicId)
            .Set(p => p.RepositoryUrl, project.RepositoryUrl)
            .Set(p => p.CreatedAt, project.CreatedAt);

        var options = new FindOneAndUpdateOptions<ProjectDocument>
        {
            ReturnDocument = ReturnDocument.After,
        };

        var updatedDoc = await _context.Projects.FindOneAndUpdateAsync(filter, update, options);

        if (updatedDoc == null)
            throw new Exception($"Projeto com Id {project.Id} não encontrado.");

        return new Project
        {
            Id = updatedDoc.Id,
            Name = updatedDoc.Name,
            Description = updatedDoc.Description,
            Skills = updatedDoc.Skills,
            ThumbnailUrl = updatedDoc.ThumbnailUrl,
            ThumbnailPublicId = updatedDoc.ThumbnailPublicId,
            
            RepositoryUrl = updatedDoc.RepositoryUrl,
            CreatedAt = updatedDoc.CreatedAt,
        };
    }

    public async Task DeleteAsync(string id)
    {
        var result = await _context.Projects.DeleteOneAsync(p => p.Id == id);
        if (result.DeletedCount == 0)
            throw new Exception($"Projeto com Id {id} não encontrado.");
    }
}
