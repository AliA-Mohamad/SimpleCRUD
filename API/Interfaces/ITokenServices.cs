using API.Models;
using System.Security.Claims;

namespace API.Interfaces;

public interface ITokenServices
{
    string GenerateToken(UserModel user);
    UserModel GetUserByToken(ClaimsPrincipal Token);
}
