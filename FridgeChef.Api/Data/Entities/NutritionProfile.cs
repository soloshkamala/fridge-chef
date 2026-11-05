
namespace FridgeChef.Api.Data.Entities;

public class NutritionProfile
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int Age { get; set; }

    public decimal Height { get; set; }

    public decimal Weight { get; set; }

    public string ActivityLevel { get; set; } = "";

    public string Goal { get; set; } = "";

    public User User { get; set; } = null!;
}
