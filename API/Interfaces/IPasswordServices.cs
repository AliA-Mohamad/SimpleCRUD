namespace API.Interfaces;

public interface IPasswordServices
{
    string GenerateHash(string password);

    bool CheckHash(string Password, string Hash);
}
