using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Interfaces;
using API.Models;
using API.Models.DTOs.Wish;
using System.Data;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WishController : ControllerBase
{
    private readonly AppDbContext _Db;
    private readonly ITokenServices _Token;

    public WishController(AppDbContext db, ITokenServices token)
    {
        _Db = db;
        _Token = token;
    }

    [Authorize]
    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateDto Dto)
    {
        var user = _Token.GetUserByToken(User);

        var Wish = new WishModel()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Wish = Dto.Wish,
            Status = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _Db.Wish.Add(Wish);
        await _Db.SaveChangesAsync();

        return Ok("Wish has created.");

    }

    [Authorize]
    [HttpGet("Get")]
    public IActionResult Get()
    {
        var user = _Token.GetUserByToken(User);

        var wishes = _Db.Wish
            .Where(w => w.UserId == user.Id)
            .Select(w => new
            {
                w.Id,
                w.UserId,
                w.Wish,
                w.Status,
                w.CreatedAt,
                w.UpdatedAt
            });

        return Ok(wishes);
    }

    [Authorize]
    [HttpPost("Update")]
    public IActionResult Update([FromBody] UpdateDto Dto)
    {
        var user = _Token.GetUserByToken(User);
        var wish = _Db.Wish.Find(Dto.Id);

        if (wish == null || user.Id != wish.UserId)
            return Unauthorized();

        wish.Wish = Dto.Wish;
        wish.UpdatedAt = DateTime.UtcNow;
        _Db.SaveChanges();

        return Ok("Wish has updated");
    }    

    [Authorize]
    [HttpGet("GetById")]
    public IActionResult GetById([FromQuery] GetByIdDto Dto)
    {
        var user = _Token.GetUserByToken(User);
        var wish = _Db.Wish.Find(Dto.Id);

        if (wish == null || user.Id != wish.UserId)
            return Unauthorized();

        var response = new
        {
            wish.Id,
            UserName = wish.User.Name,
            wish.Wish,
            wish.Status,
            wish.CreatedAt,
            wish.UpdatedAt
        };

        return Ok(response);
    }

    [Authorize]
    [HttpDelete("Delete")]
    public IActionResult Delete([FromBody] DeleteDto Dto)
    {
        var user = _Token.GetUserByToken(User);
        var wish = _Db.Wish.Find(Dto.Id);

        if (wish == null || user.Id != wish.UserId)
            return Unauthorized();

        _Db.Wish.Remove(wish);
        _Db.SaveChanges();

        return Ok("Whis has Removed");
    }
}
