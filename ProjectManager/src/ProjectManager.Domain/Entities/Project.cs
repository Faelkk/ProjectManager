public class Project
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<string> Skills { get; set; } = new List<string>();
    public string RepositoryUrl { get; set; } = null!;
    public string? ThumbnailUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
