using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectManager.Infrastructure.Models;

public class ProjectDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    [BsonElement("name")]
    public string Name { get; set; } = default!;

    [BsonElement("description")]
    public string Description { get; set; } = default!;

    [BsonElement("skills")]
    public List<string> Skills { get; set; } = new();

    [BsonElement("avatarUrl")]
    public string? AvatarUrl { get; set; }
    
    [BsonElement("avatarPublicId")]
    public string? AvatarPublicId { get; set; }
    
    [BsonElement("repositoryUrl")]
    public string RepositoryUrl { get; set; } = default!;

    [BsonElement("thumbnailUrl")]
    public string? ThumbnailUrl { get; set; }
    
    
    [BsonElement("thumbnailPublicId")]
    public string? ThumbnailPublicId { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
