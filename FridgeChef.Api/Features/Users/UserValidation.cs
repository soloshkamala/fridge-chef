
using System.Net.Mail;

namespace FridgeChef.Api.Features.Users;

public static class UserValidation
{
    public static string? Validate(
        string? email,
        string? password,
        string? displayName)
    {
        if (string.IsNullOrWhiteSpace(email))
            return "Email is required.";

        if (email.Length > 254 ||
            !MailAddress.TryCreate(email, out var address) ||
            address.Address != email ||
            !email.Contains('.'))
            return "Invalid email format.";

        if (string.IsNullOrWhiteSpace(password) ||
            password.Length < 8)
            return "Password must contain at least 8 characters.";

        if (string.IsNullOrWhiteSpace(displayName) ||
            displayName.Length > 100)
            return "Display name must contain 1 to 100 characters.";

        return null;
    }
}
