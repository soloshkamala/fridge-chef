using System.ComponentModel.DataAnnotations;

namespace FridgeChef.Models;

public class ProfileForm
{
    [Required(ErrorMessage = "Вкажіть ім'я")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Вкажіть email")]
    [EmailAddress(ErrorMessage = "Некоректний email")]
    public string? Email { get; set; }

    public IReadOnlyCollection<string> Diets { get; set; } = new HashSet<string>();
    public string? FavoriteIngredients { get; set; }
}