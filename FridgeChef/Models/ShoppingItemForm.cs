using System.ComponentModel.DataAnnotations;

namespace FridgeChef.Models
{
    public class ShoppingItemForm
    {
        [Required(ErrorMessage = "Введіть назву продукту")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть кількість")]
        [Range(0.01, 1000, ErrorMessage = "Кількість має бути більшою за 0")]
        public double? Quantity { get; set; }

        [Required(ErrorMessage = "Оберіть одиницю вимірювання")]
        public string Unit { get; set; } = string.Empty;

        [Required(ErrorMessage = "Оберіть категорію")]
        public string Category { get; set; } = string.Empty;
    }
}