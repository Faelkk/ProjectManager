using MongoDB.Driver;
using ProjectManager.Infrastructure.Context;
using ProjectManager.Infrastructure.Mongo;

public class UserRepository : IUserRepository
{
    private readonly IDatabaseContext _context;

    public UserRepository(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var documents = await _context.Users.Find(_ => true).ToListAsync();

        return documents.Select(d => new User
        {
            Id = d.Id,
            Username = d.Username,
            Email = d.Email,
            Role = d.Role,
        });
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var document = await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
        if (document == null)
            return null;

        return new User
        {
            Id = document.Id,
            Username = document.Username,
            Email = document.Email,
            Role = document.Role,
            Password = document.Password,
        };
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        var document = await _context.Users.Find(u => u.Id == id).FirstOrDefaultAsync();
        if (document == null)
            return null;

        return new User
        {
            Id = document.Id,
            Username = document.Username,
            Email = document.Email,
            Role = document.Role,
        };
    }

    public async Task<User> CreateAsync(User user)
    {
        var document = new UserDocument
        {
            Username = user.Username,
            Email = user.Email,
            Password = user.Password,
        };

        await _context.Users.InsertOneAsync(document);
        user.Id = document.Id;

        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        var filter = Builders<UserDocument>.Filter.Eq(u => u.Id, user.Id);

        var update = Builders<UserDocument>
            .Update.Set(u => u.Username, user.Username)
            .Set(u => u.Email, user.Email)
            .Set(u => u.Role, user.Role);

        var options = new FindOneAndUpdateOptions<UserDocument>
        {
            ReturnDocument = ReturnDocument.After,
        };

        var updatedDoc = await _context.Users.FindOneAndUpdateAsync(filter, update, options);

        if (updatedDoc == null)
            throw new Exception($"Usuário com Id {user.Id} não encontrado.");

        return new User
        {
            Id = updatedDoc.Id,
            Username = updatedDoc.Username,
            Email = updatedDoc.Email,
            Role = updatedDoc.Role,
        };
    }

    public async Task DeleteAsync(string id)
    {
        var result = await _context.Users.DeleteOneAsync(p => p.Id == id);
        if (result.DeletedCount == 0)
            throw new Exception($"Usuário com Id {id} não encontrado.");
    }
}
