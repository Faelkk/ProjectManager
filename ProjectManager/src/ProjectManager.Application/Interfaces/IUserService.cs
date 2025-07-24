public interface IUserService
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task<string> CreateAsync(User user);

    Task<string> LoginAsync(string email, string password);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(string id);
}
