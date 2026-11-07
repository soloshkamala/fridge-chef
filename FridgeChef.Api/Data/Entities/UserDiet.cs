namespace FridgeChef.Api.Data.Entities;

public class UserDiet
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int DietId { get; set; }
    public Diet Diet { get; set; } = null!;
}