namespace API.Models;

public class WishModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Wish { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public UserModel User { get; set; }
}
