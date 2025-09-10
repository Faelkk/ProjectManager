using HotChocolate.Authorization;
using ProjectManager.Application.DTOS;
using ProjectManager.Application.Interfaces;

[ExtendObjectType("Mutation")]
public class ProjectMutation
{
    
    [Authorize(Policy = "Authenticated")]
    [Authorize(Policy = "Admin")]
    public async Task<Project> CreateProject(
        string name,
        string description,
        List<string> skills,
        string repositoryUrl,
        IFile? thumbnail, // 
        [Service] IProjectService service,
        [Service] ICloudinaryService cloudinaryService
    )
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new GraphQLException("O nome do projeto é obrigatório.");

        if (string.IsNullOrWhiteSpace(description))
            throw new GraphQLException("A descrição do projeto é obrigatória.");

        if (skills == null || skills.Count == 0)
            throw new GraphQLException("Informe pelo menos uma skill.");

        if (string.IsNullOrWhiteSpace(repositoryUrl))
            throw new GraphQLException("O URL do repositório é obrigatório.");
        
        CloudinaryUploadResult? thumbnailUrl = null;
        if (thumbnail != null)
        {
            using var stream = thumbnail.OpenReadStream();
            thumbnailUrl = await cloudinaryService.UploadImage(thumbnail.Name, stream);
        }

        var project = new Project
        {
            Name = name,
            Description = description,
            Skills = skills,
            ThumbnailUrl = thumbnailUrl.Url,
            RepositoryUrl = repositoryUrl,
            CreatedAt = DateTime.UtcNow,
        };

        return await service.CreateAsync(project);
    }



  [Authorize(Policy = "Authenticated")]
[Authorize(Policy = "Admin")]
public async Task<Project> UpdateProject(
    string id,
    string? name,
    string? description,
    List<string>? skills,
    string? repositoryUrl,
    IFile? thumbnail, 
    [Service] IProjectService service,
    [Service] ICloudinaryService cloudinaryService
)
{
    var existingProject = await service.GetByIdAsync(id);
    if (existingProject == null)
        throw new GraphQLException($"Projeto com id '{id}' não encontrado.");

    if (!string.IsNullOrWhiteSpace(name))
        existingProject.Name = name;

    if (!string.IsNullOrWhiteSpace(description))
        existingProject.Description = description;

    if (skills != null && skills.Count > 0)
        existingProject.Skills = skills;

    if (!string.IsNullOrWhiteSpace(repositoryUrl))
        existingProject.RepositoryUrl = repositoryUrl;
    
    
    if (thumbnail != null)
    {
        using var stream = thumbnail.OpenReadStream();
        var newUrl = await cloudinaryService.UploadImage(thumbnail.Name, stream);
        existingProject.ThumbnailUrl = newUrl.Url;
        existingProject.ThumbnailPublicId = newUrl.PublicId;
    }

    return await service.UpdateAsync(existingProject);
}


[Authorize(Policy = "Authenticated")]
[Authorize(Policy = "Admin")]
public async Task<bool> DeleteProject(string id, [Service] IProjectService service, [Service] ICloudinaryService cloudinaryService)
{
    var existingProject = await service.GetByIdAsync(id);
    if (existingProject == null)
        throw new GraphQLException($"Projeto com id '{id}' não encontrado.");
    
    if (!string.IsNullOrWhiteSpace(existingProject.ThumbnailPublicId))
    {
        await cloudinaryService.DeleteFileAsync(existingProject.ThumbnailPublicId);
    }

    await service.DeleteAsync(id);
    return true;
}

}


