
using FridgeChef.Models;

namespace FridgeChef.Services
{
    public class FridgeService
    {
        public bool IsLoading { get; set; } = false;
        public string? ErrorMessage { get; set; }

        public List<Ingredient> Ingredients { get; set; } = new();

        public List<FridgeItem> FridgeItems { get; } = new();

        public void SetIngredients(List<ProductResponse> products)
        {
            Ingredients = products.Select(product => new Ingredient
            {
                Id = product.Id,
                Name = product.Name,
                AllowedUnits = GetAllowedUnits(product.Name)
            }).ToList();
        }

        private List<string> GetAllowedUnits(string name)
        {
            return name.ToLower() switch
            {
                "яйця" => new() { "шт" },
                "молоко" => new() { "мл", "л" },
                "ковбаса" => new() { "г", "кг" },
                "сир" => new() { "г", "кг" },
                "помідори" => new() { "шт", "г", "кг" },
                "огірки" => new() { "шт", "г", "кг" },
                "картопля" => new() { "шт", "г", "кг" },
                "масло" => new() { "г" },
                _ => new() { "шт", "г", "кг" }
            };
        }

        public void AddToFridge(Ingredient ingredient, double quantity, string unit)
        {
            if (quantity <= 0) return;
            if (!ingredient.AllowedUnits.Contains(unit)) return;

            if (unit == "кг")
            {
                quantity *= 1000;
                unit = "г";
            }
            else if (unit == "л")
            {
                quantity *= 1000;
                unit = "мл";
            }

            var existingItem = FridgeItems.FirstOrDefault(item =>
                item.Ingredient.Id == ingredient.Id &&
                item.Unit == unit);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                FridgeItems.Add(new FridgeItem
                {
                    Id = FridgeItems.Count == 0
                        ? 1
                        : FridgeItems.Max(item => item.Id) + 1,
                    Ingredient = ingredient,
                    Quantity = quantity,
                    Unit = unit
                });
            }
        }
    }
}
