namespace FridgeChef.Api.Data.Entities;

public class UserProfile
{
    public int Id { get; set; }

    public int UserId { get; set; }



    public User User { get; set; } = null!;

  
}