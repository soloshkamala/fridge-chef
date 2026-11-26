namespace FridgeChef.Models
{
    public class Ingredient
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string DisplayName { get; set; } = "";

        public List<string> AllowedUnits { get; set; } = new();

        public Dictionary<string, string> DisplayUnits { get; set; } = new();

        public string LocalizedName =>
            string.IsNullOrWhiteSpace(DisplayName)
                ? Name
                : DisplayName;

        public string GetLocalizedUnit(string unit)
        {
            return DisplayUnits.TryGetValue(unit, out var translated)
                && !string.IsNullOrWhiteSpace(translated)
                ? translated
                : unit;
        }
    }
}
