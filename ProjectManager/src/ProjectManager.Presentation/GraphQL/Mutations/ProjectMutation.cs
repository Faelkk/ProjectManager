using HotChocolate.Authorization;
using ProjectManager.Application.DTOS;
using ProjectManager.Application.FileValidator;
using ProjectManager.Application.Interfaces;
using ProjectManager.API.GraphQL.Adapters;
using ProjectManager.Application.Common.Interfaces;

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
        IFile? thumbnail, 
        IFile? avatar, 
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
            var appFile = new HotChocolateFileAdapter(thumbnail);
            FileValidator.ValidateImage(appFile);

            using var stream = appFile.OpenReadStream();
            thumbnailUrl = await cloudinaryService.UploadImage(appFile.Name, stream);
        }

        CloudinaryUploadResult? avatarUrl = null;
        if (avatar != null)
        {
            var appFile = new HotChocolateFileAdapter(avatar);
            FileValidator.ValidateImage(appFile);

            using var stream = appFile.OpenReadStream();
            avatarUrl = await cloudinaryService.UploadImage(appFile.Name, stream);
        }
        var project = new Project
        {
            Name = name,
            Description = description,
            Skills = skills,
            RepositoryUrl = repositoryUrl,
            CreatedAt = DateTime.UtcNow,
            ThumbnailUrl = thumbnailUrl?.Url,
            ThumbnailPublicId = thumbnailUrl?.PublicId,
            AvatarUrl = avatarUrl?.Url,
            AvatarPublicId = avatarUrl?.PublicId
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
    IFile? avatar, 
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
        if (!string.IsNullOrWhiteSpace(existingProject.ThumbnailPublicId))
            await cloudinaryService.DeleteFileAsync(existingProject.ThumbnailPublicId);

        var appFile = new HotChocolateFileAdapter(thumbnail);
        FileValidator.ValidateImage(appFile);

        using var stream = appFile.OpenReadStream();
        var newThumbnail = await cloudinaryService.UploadImage(appFile.Name, stream);

        existingProject.ThumbnailUrl = newThumbnail.Url;
        existingProject.ThumbnailPublicId = newThumbnail.PublicId;
    }
    
    if (avatar != null)
    {
        if (!string.IsNullOrWhiteSpace(existingProject.AvatarPublicId))
            await cloudinaryService.DeleteFileAsync(existingProject.AvatarPublicId);

        var appFile = new HotChocolateFileAdapter(avatar);
        FileValidator.ValidateImage(appFile);

        using var stream = appFile.OpenReadStream();
        var newAvatar = await cloudinaryService.UploadImage(appFile.Name, stream);

        existingProject.AvatarUrl = newAvatar.Url;
        existingProject.AvatarPublicId = newAvatar.PublicId;
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

    if (!string.IsNullOrWhiteSpace(existingProject.AvatarPublicId))
    {
        await cloudinaryService.DeleteFileAsync(existingProject.AvatarPublicId);
    }
    await service.DeleteAsync(id);
    return true;
}
}


