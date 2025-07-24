using Microsoft.AspNetCore.Identity;

public class PasswordHasher : IPasswordHasher
{
    private readonly Microsoft.AspNetCore.Identity.PasswordHasher<object> _hasher = new();

    public string HashPassword(User user, string password)
    {
        return _hasher.HashPassword(user, password);
    }

    public bool VerifyPassword(User user, string hash, string password)
    {
        return _hasher.VerifyHashedPassword(user, hash, password)
            != PasswordVerificationResult.Failed;
    }
}
