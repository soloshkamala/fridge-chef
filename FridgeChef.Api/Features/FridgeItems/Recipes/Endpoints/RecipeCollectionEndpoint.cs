
using System.Security.Claims;
using FridgeChef.Api.Data;
using FridgeChef.Api.Data.Entities;
using FridgeChef.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace FridgeChef.Api.Features.Recipes.Endpoints;

public static class RecipeCollectionEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}/collection", async (
            int id,
            ClaimsPrincipal user,
            CurrentUserService currentUserService,
            FridgeChefDbContext db,
            CancellationToken ct) =>
        {
            var userId = await currentUserService.GetUserIdAsync(user);

            if (userId == null)
                return Results.Unauthorized();

            var exists = await db.Recipes
                .AnyAsync(r => r.Id == id, ct);

            if (!exists)
                return Results.NotFound();

            var collection = await db.UserRecipeCollections
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.UserId == userId.Value &&
                         x.RecipeId == id, ct);

            return Results.Ok(new
            {
                IsSaved = collection?.IsSaved ?? false,
                IsFavorite = collection?.IsFavorite ?? false
            });
        });


        group.MapPut("/{id:int}/collection/{type}", async (
            int id,
            string type,
            CollectionUpdateRequest request,
            ClaimsPrincipal user,
            CurrentUserService currentUserService,
            FridgeChefDbContext db,
            CancellationToken ct) =>
        {
            if (type != "saved" && type != "favorite")
                return Results.BadRequest("Invalid collection type.");

            var userId = await currentUserService.GetUserIdAsync(user);

            if (userId == null)
                return Results.Unauthorized();

            var exists = await db.Recipes
                .AnyAsync(r => r.Id == id, ct);

            if (!exists)
                return Results.NotFound();

            var collection = await db.UserRecipeCollections
                .FirstOrDefaultAsync(
                    x => x.UserId == userId.Value &&
                         x.RecipeId == id, ct);

            if (collection == null)
            {
                if (!request.Value)
                    return Results.NoContent();

                collection = new UserRecipeCollection
                {
                    UserId = userId.Value,
                    RecipeId = id
                };

                db.UserRecipeCollections.Add(collection);
            }

            if (type == "saved")
                collection.IsSaved = request.Value;
            else
                collection.IsFavorite = request.Value;

            if (!collection.IsSaved && !collection.IsFavorite)
                db.UserRecipeCollections.Remove(collection);

            await db.SaveChangesAsync(ct);

            return Results.NoContent();
        });
    }

    public record CollectionUpdateRequest(bool Value);
}

