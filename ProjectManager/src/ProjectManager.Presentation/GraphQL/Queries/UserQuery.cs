using HotChocolate.Authorization;

[ExtendObjectType("Query")]
[Authorize(Policy = "Authenticated")]
public class UserQuery
{
    public Task<IEnumerable<User>> GetUsers([Service] IUserService service) =>
        service.GetAllAsync();

    public Task<User?> GetUserById(string id, [Service] IUserService service) =>
        service.GetByIdAsync(id);
}
