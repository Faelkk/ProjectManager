using MongoDB.Driver;
using ProjectManager.Infrastructure.Models;
using ProjectManager.Infrastructure.Mongo;

namespace ProjectManager.Infrastructure.Context;

public interface IDatabaseContext
{
    IMongoCollection<ProjectDocument> Projects { get; }
    IMongoCollection<UserDocument> Users { get; }
}
