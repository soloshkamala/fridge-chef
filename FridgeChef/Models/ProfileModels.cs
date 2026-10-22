namespace FridgeChef.Models;

public enum ProfileViewState { Loading, Empty, Error, Data }

public record RecipeSummary(string Id, string Title, string ImageUrl, int Minutes, int Kcal);

public record UserProfile(
    string Name, string Email, string Badge, string? AvatarUrl,
    DateOnly? JoinedAt, int SavedCount, int FavoriteCatalogs);

public record ProfileData(UserProfile User, IReadOnlyList<RecipeSummary> Recipes);