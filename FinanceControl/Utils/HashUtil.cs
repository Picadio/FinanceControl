using System.Security.Cryptography;

namespace FinanceControl.Utils;

public static class HashUtil
{
    private const int SaltSize = 16;
    private const int KeySize = 32; 
    private const int Iterations = 100_000;
    
    public static string Hash(string data)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);
        
        using var pbkdf2 = new Rfc2898DeriveBytes(data, salt, Iterations, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(KeySize);
        
        var result = Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash) + "." + Iterations;
        return result;
    }
    
    public static bool VerifyHash(string storedHash, string data)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 3) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var hash = Convert.FromBase64String(parts[1]);
        var iterations = int.Parse(parts[2]);

        using var pbkdf2 = new Rfc2898DeriveBytes(data, salt, iterations, HashAlgorithmName.SHA256);
        var inputHash = pbkdf2.GetBytes(KeySize);
        
        return CryptographicOperations.FixedTimeEquals(hash, inputHash);
    }
}