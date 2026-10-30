namespace FridgeChef.Api.Features.Profiles;

public record ProfileDto(
    int Id,
    string Name,
    string Email,
    IReadOnlyCollection<string> Diets,
    string FavoriteIngredients
);

public record CreateProfileRequest(
    string Name,
    string Email,
    IReadOnlyCollection<string> Diets,
    string FavoriteIngredients
);

public record UpdateProfileRequest(
    string Name,
    string Email,
    IReadOnlyCollection<string> Diets,
    string FavoriteIngredients
);