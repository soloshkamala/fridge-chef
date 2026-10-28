namespace FridgeChef.Models
{
    public class Ingredient
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public List<string> AllowedUnits { get; set; } = new();
    }
}
