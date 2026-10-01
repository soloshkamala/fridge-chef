using FridgeChef.Models;

namespace FridgeChef.Services
{
    public class FridgeService
    {
        public List<Ingredient> Ingredients { get; } = new()
        {
            new Ingredient
            {
                Id = 1,
                Name = "Яйця",
                AllowedUnits = new() { "шт" }
            },

            new Ingredient
            {
                Id = 2,
                Name = "Молоко",
                AllowedUnits = new() { "мл", "л" }
            },

            new Ingredient
            {
                Id = 3,
                Name = "Ковбаса",
                AllowedUnits = new() { "г", "кг" }
            },

            new Ingredient
            {
                Id = 4,
                Name = "Сир",
                AllowedUnits = new() { "г", "кг" }
            },

            new Ingredient
            {
                Id = 5,
                Name = "Помідори",
                AllowedUnits = new() { "шт", "г", "кг" }
            },

            new Ingredient
            {
                Id = 6,
                Name = "Огірки",
                AllowedUnits = new() { "шт", "г", "кг" }
            },

            new Ingredient
            {
                Id = 7,
                Name = "Картопля",
                AllowedUnits = new() { "шт", "г", "кг" }
            },

            new Ingredient
            {
                Id = 8,
                Name = "Масло",
                AllowedUnits = new() { "г" }
            }
        };

        public List<FridgeItem> FridgeItems { get; } = new();

        public void AddToFridge(Ingredient ingredient, double quantity, string unit)
        {
            FridgeItems.Add(new FridgeItem
            {
                Id = FridgeItems.Count + 1,
                Ingredient = ingredient,
                Quantity = quantity,
                Unit = unit
            });
        }
    }
}