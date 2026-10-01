namespace FridgeChef.Models
{
    public class FridgeItem
    {
        public int Id { get; set; }

        public Ingredient Ingredient { get; set; } = new();

        public double Quantity { get; set; }

        public string Unit { get; set; } = "";
    }
}