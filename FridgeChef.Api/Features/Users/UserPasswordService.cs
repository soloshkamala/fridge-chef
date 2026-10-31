
using Microsoft.AspNetCore.Identity;

namespace FridgeChef.Api.Features.Users;

public class UserPasswordService
{
    private readonly PasswordHasher<User> hasher = new();

    public string Hash(User user, string password)
    {
        return hasher.HashPassword(user, password);
    }

    public bool Verify(User user, string password)
    {
        var result = hasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            password);

        return result != PasswordVerificationResult.Failed;
    }
}
