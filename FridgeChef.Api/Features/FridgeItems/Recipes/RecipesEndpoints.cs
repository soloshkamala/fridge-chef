using FridgeChef.Api.Features.Recipes.Endpoints;

namespace FridgeChef.Api.Features.Recipes;

public static class RecipesEndpoints
{
    public static RouteGroupBuilder MapRecipesEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/recipes")
            .WithTags("Recipes");

        GetRecommendedRecipesEndpoint.Map(group);

        return group;
    }
}