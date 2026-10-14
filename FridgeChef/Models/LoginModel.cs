
using System.ComponentModel.DataAnnotations;

namespace FridgeChef.Models;

public class LoginModel
{
    [Required(ErrorMessage = "Введіть електронну пошту")]
    [EmailAddress(ErrorMessage = "Некоректна електронна пошта")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Введіть пароль")]
    public string Password { get; set; } = "";

    public bool RememberMe { get; set; }
}
