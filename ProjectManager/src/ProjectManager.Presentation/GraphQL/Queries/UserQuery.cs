using HotChocolate.Authorization;

[ExtendObjectType("Query")]
public class UserQuery
{
    [Authorize(Policy = "Authenticated")]
    public Task<IEnumerable<User>> GetUsers([Service] IUserService service) =>
        service.GetAllAsync();

    [Authorize(Policy = "Authenticated")]
    public Task<User?> GetUserById([GraphQLName("id")] string id, [Service] IUserService service) =>
        service.GetByIdAsync(id);
}
