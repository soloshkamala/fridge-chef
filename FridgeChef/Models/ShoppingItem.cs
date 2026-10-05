namespace FridgeChef.Models
{
    public class ShoppingItem
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public double Quantity { get; set; }

        public string Unit { get; set; } = "";

        public string Category { get; set; } = "";

        public bool IsPurchased { get; set; }

        public bool IsInFridge { get; set; }
    }
}