using HotChocolate.Authorization;

[ExtendObjectType("Mutation")]
public class UserMutation
{
    public async Task<UserResponseTokenDto> CreateUser(
        string username,
        string email,
        string password,
        [Service] IUserService service
    )
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new GraphQLException("O username do usuário é obrigatório.");

        if (string.IsNullOrWhiteSpace(email))
            throw new GraphQLException("O email do usuário é obrigatório.");

        if (string.IsNullOrWhiteSpace(password))
            throw new GraphQLException("A senha do usuário é obrigatória.");

        var user = new User
        {
            Username = username,
            Email = email,
            Password = password,
            Role = "User",
        };

        var token = await service.CreateAsync(user);

        return new UserResponseTokenDto { Token = token };
    }

    public async Task<UserResponseTokenDto> Login(
        string email,
        string password,
        [Service] IUserService service
    )
    {
        var token = await service.LoginAsync(email, password);
        if (token == null)
            throw new GraphQLException("Email ou senha inválidos.");

        return new UserResponseTokenDto { Token = token };
    }

    [Authorize(Policy = "Authenticated")]
    [Authorize(Policy = "Admin")]
    public async Task<User> UpdateUser(UpdateUserInput input, [Service] IUserService service)
    {
        var existingUser = await service.GetByIdAsync(input.Id);
        if (existingUser == null)
            throw new GraphQLException($"Usuário com id '{input.Id}' não encontrado.");

        if (input.Username != null)
            existingUser.Username = input.Username;

        if (input.Email != null)
            existingUser.Email = input.Email;

        if (input.Role != null)
            existingUser.Role = input.Role;

        return await service.UpdateAsync(existingUser);
    }

    [Authorize(Policy = "Authenticated")]
    [Authorize(Policy = "Admin")]
    public async Task<bool> DeleteUser(string id, [Service] IUserService service)
    {
        var existingUser = await service.GetByIdAsync(id);
        if (existingUser == null)
            throw new GraphQLException($"Usuário com id '{id}' não encontrado.");

        await service.DeleteAsync(id);
        return true;
    }
}
