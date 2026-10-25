
using Xunit;
using FridgeChef.Services;

namespace FridgeChef.Tests;

public class AuthServiceTests
{
    [Fact]
    public void Login_WithCorrectCredentials_ReturnsTrue()
    {
        var service = new AuthService();

        bool result = service.Login(
            "student@example.com",
            "password123"
        );

        Assert.True(result);
    }

    [Fact]
    public void Login_WithWrongCredentials_ReturnsFalse()
    {
        var service = new AuthService();

        bool result = service.Login(
            "wrong@example.com",
            "wrongpassword"
        );

        Assert.False(result);
    }
}
