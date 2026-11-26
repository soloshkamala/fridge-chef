namespace FridgeChef.Models
{
    public class ProductResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string DisplayName { get; set; } = "";

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = "";

        public string DisplayCategoryName { get; set; } = "";

        public List<string> AllowedUnits { get; set; } = new();

        public Dictionary<string, string> DisplayUnits { get; set; } = new();
    }
}
