
namespace FridgeChef.Api.Features.Auth;

public record LoginRequest(
    string Email,
    string Password
);

public record AuthResponse(
    int Id,
    string Email,
    string DisplayName
);
