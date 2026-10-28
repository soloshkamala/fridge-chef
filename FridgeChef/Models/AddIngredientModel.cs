using System.ComponentModel.DataAnnotations;

namespace FridgeChef.Models
{
    public class AddIngredientModel
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "Кількість повинна бути більшою за нуль")]
        public double Quantity { get; set; } = 1;

        [Required(ErrorMessage = "Оберіть одиницю вимірювання")]
        public string? Unit { get; set; }
    }
}
