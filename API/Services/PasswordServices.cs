using API.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace API.Services;

public class PasswordServices : IPasswordServices
{

    public string GenerateHash(string password)
    {
        var sha256 = SHA256.Create();
        var builder = new StringBuilder();

        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password)); 
        foreach (var b in bytes)
            builder.Append(b.ToString("x2"));

        return builder.ToString();
    }

    public bool CheckHash(string Password, string Hash)
    {
        string HashPassword = GenerateHash(Password);
        return Hash == HashPassword;
    }
}
