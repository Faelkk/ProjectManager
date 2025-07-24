using HotChocolate.Authorization;
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
        string? thumbnailUrl,
        string repositoryUrl,
        [Service] IProjectService service
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

        var project = new Project
        {
            Name = name,
            Description = description,
            Skills = skills,
            ThumbnailUrl = thumbnailUrl,
            RepositoryUrl = repositoryUrl,
            CreatedAt = DateTime.UtcNow,
        };

        return await service.CreateAsync(project);
    }

    [Authorize(Policy = "Authenticated")]
    [Authorize(Policy = "Admin")]
    public async Task<Project> UpdateProject(
        UpdateProjectInput input,
        [Service] IProjectService service
    )
    {
        var existingProject = await service.GetByIdAsync(input.Id);
        if (existingProject == null)
            throw new GraphQLException($"Projeto com id '{input.Id}' não encontrado.");

        if (input.Name != null)
            existingProject.Name = input.Name;

        if (input.Description != null)
            existingProject.Description = input.Description;

        if (input.Skills != null)
            existingProject.Skills = input.Skills;

        if (input.ThumbnailUrl != null)
            existingProject.ThumbnailUrl = input.ThumbnailUrl;

        if (input.RepositoryUrl != null)
            existingProject.RepositoryUrl = input.RepositoryUrl;

        return await service.UpdateAsync(existingProject);
    }

    [Authorize(Policy = "Authenticated")]
    [Authorize(Policy = "Admin")]
    public async Task<bool> DeleteProject(string id, [Service] IProjectService service)
    {
        var existingProject = await service.GetByIdAsync(id);
        if (existingProject == null)
            throw new GraphQLException($"Projeto com id '{id}' não encontrado.");

        await service.DeleteAsync(id);
        return true;
    }
}
