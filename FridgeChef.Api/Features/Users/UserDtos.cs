
namespace FridgeChef.Api.Features.Users;

public record CreateUserRequest(
    string Email,
    string Password,
    string DisplayName
);

public record ReplaceUserRequest(
    string Email,
    string Password,
    string DisplayName
);

public record UserResponse(
    int Id,
    string Email,
    string DisplayName
);
