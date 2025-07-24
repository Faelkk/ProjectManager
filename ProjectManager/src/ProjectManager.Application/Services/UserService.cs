public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    private readonly ITokenService _tokenService;

    public UserService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService
    )
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<string> CreateAsync(User user)
    {
        user.Password = _passwordHasher.HashPassword(user, user.Password);

        var userExists = await _userRepository.GetByEmailAsync(user.Email);
        if (userExists != null)
        {
            throw new ArgumentException("Email já está em uso.");
        }

        var userCreated = await _userRepository.CreateAsync(user);

        var token = _tokenService.GenerateToken(userCreated);

        return token;
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !_passwordHasher.VerifyPassword(user, user.Password, password))
        {
            throw new ArgumentException("Email ou senha inválidos.");
        }

        var token = _tokenService.GenerateToken(user);

        return token;
    }

    public async Task<User> UpdateAsync(User user)
    {
        return await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteAsync(string id)
    {
        await _userRepository.DeleteAsync(id);
    }
}
