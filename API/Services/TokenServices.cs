
using Microsoft.IdentityModel.Tokens;
using API.Data;
using API.Interfaces;
using API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services;

public class TokenServices : ITokenServices
{
    private readonly IConfiguration _Configuration;
    private readonly AppDbContext _Db;

    public TokenServices(IConfiguration configuration, AppDbContext db)
    {
        _Configuration = configuration;
        _Db = db;
    }

    public string GenerateToken(UserModel user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_Configuration["JwtConfig:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public UserModel GetUserByToken(ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        return _Db.Users.Find(Guid.Parse(userId));
    }
}
