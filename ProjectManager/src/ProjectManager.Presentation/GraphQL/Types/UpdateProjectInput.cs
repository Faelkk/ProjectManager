public class UpdateProjectInput
{
    public string Id { get; set; } = null!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<string>? Skills { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? RepositoryUrl { get; set; }
}
