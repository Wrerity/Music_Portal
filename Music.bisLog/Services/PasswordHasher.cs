using System.Security.Cryptography;

namespace Music.bisLog.Services;

public class PasswordHasher
{
    private const int SaltSize = 32;

    public (string hash, string salt) Hash(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
        var salt = Convert.ToBase64String(saltBytes);

        using var sha256 = SHA256.Create();
        var combined = $"{password}{salt}";
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(combined));
        var hash = Convert.ToBase64String(hashBytes);

        return (hash, salt);
    }

    public bool Verify(string password, string hash, string salt)
    {
        using var sha256 = SHA256.Create();
        var combined = $"{password}{salt}";
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(combined));
        var computedHash = Convert.ToBase64String(hashBytes);

        return computedHash == hash;
    }
}
