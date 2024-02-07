namespace API.Models;

public class UserModel
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdate { get; set; }

    public List<WishModel> Wish { get; set; }
}
