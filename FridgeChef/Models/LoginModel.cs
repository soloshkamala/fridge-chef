
using System.ComponentModel.DataAnnotations;

namespace FridgeChef.Models;

public class LoginModel
{
    [Required(ErrorMessage = "Введіть електронну пошту")]
    [EmailAddress(ErrorMessage = "Неправильний формат Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введіть пароль")]
    public string Password { get; set; } = string.Empty;
}
