using System.Security.Cryptography;
using System.Text;

namespace Controle_Pessoal.Auth;

public class PasswordHasher
{
    public static string HashPassword(string password)
    {
        using SHA256 sha256 = SHA256.Create();
        
        byte[] bytes = Encoding.UTF8.GetBytes(password);
        byte[] hashBytes = SHA256.HashData(bytes);

        var hashStringBuilder = new StringBuilder();
        foreach (byte b in hashBytes)
        {
            hashStringBuilder.Append(b.ToString("x2")); // Format to hex string
        }
        
        return hashStringBuilder.ToString();
    }
    
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        string hashedInputPassword = HashPassword(password);
        return hashedInputPassword.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase);
    }
}
