using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Interfaces;
using API.Models;
using API.Models.DTOs.User;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    readonly AppDbContext _Db;
    readonly IPasswordServices _passwordServices;
    readonly ITokenServices _Token;

    public UserController(AppDbContext db, IPasswordServices passwordServices, ITokenServices tokenServices)
    {
        _Db = db;
        _passwordServices = passwordServices;
        _Token = tokenServices;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto Dto)
    {
        if (_Db.Users.Any(u => u.Email == Dto.Email))
            return BadRequest("alredy used E-mail!");

        var user = new UserModel()
        {
            Id = Guid.NewGuid(),
            Email = Dto.Email,
            Name = Dto.Name,
            Password = _passwordServices.GenerateHash(Dto.Password),
            CreatedAt = DateTime.UtcNow,
            LastUpdate = DateTime.UtcNow
        };

        _Db.Users.Add(user);
        await _Db.SaveChangesAsync();

        return Ok("Successfully registered.");
    }

    [HttpGet("Login")]
    public IActionResult Login([FromQuery] LoginDto Dto)
    {
        var user = _Db.Users.FirstOrDefault(u => u.Email == Dto.Email);

        if (user == null || !_passwordServices.CheckHash(Dto.Password, user.Password))
            return BadRequest("Email or password invalid");

        var token = _Token.GenerateToken(user);
        return Ok(new { Token = token });
    }

    [Authorize]
    [HttpPost("Update")]
    public IActionResult Update([FromBody] UpdateDto Dto)
    {
        var user = _Token.GetUserByToken(User);

        if (!_passwordServices.CheckHash(Dto.Password, user.Password))
            return Unauthorized("Invalid Password");

        user.Password = _passwordServices.GenerateHash(Dto.NewPassword);
        user.LastUpdate = DateTime.UtcNow;
        _Db.SaveChanges();

        return Ok("password Updated");
    }

    [Authorize]
    [HttpDelete("Delete")]
    public IActionResult Delete([FromBody] DeleteDto Dto)
    {
        var user = _Token.GetUserByToken(User);

        if (!_passwordServices.CheckHash(Dto.Password, user.Password))
            return Unauthorized("Invalid Password");

        _Db.Users.Remove(user);
        _Db.SaveChanges();

        return Ok("User has deleted");
    }
}
