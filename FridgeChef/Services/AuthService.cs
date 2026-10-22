
namespace FridgeChef.Services;

public class AuthService
{
    public bool Login(string email, string password)
    {
        return email == "student@example.com"
            && password == "password123";
    }
}
