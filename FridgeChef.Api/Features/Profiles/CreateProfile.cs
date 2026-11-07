
using FridgeChef.Api.Data;
using FridgeChef.Api.Data.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FridgeChef.Api.Features.Profiles;

public static class CreateProfile
{
    public static RouteHandlerBuilder MapCreateProfile(this RouteGroupBuilder group)
    {
        return group.MapPost("/", async Task<Results<Created<ProfileDto>, ValidationProblem, Conflict<string>>>
            (CreateProfileRequest request, FridgeChefDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["Email"] = ["Email є обов'язковим."]
                });
            }

            if (request.Diets is null || request.Diets.Any(string.IsNullOrWhiteSpace))
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["Diets"] = ["Назви дієт не можуть бути порожніми."]
                });
            }

            var email = request.Email.Trim();

            
            var user = await db.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user is null)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["Email"] = ["Користувача з таким email не знайдено."]
                });
            }

            if (await db.UserProfiles.AnyAsync(p => p.UserId == user.Id))
            {
                return TypedResults.Conflict(
                    "Профіль для цього користувача вже існує.");
            }

            var dietNames = request.Diets
                .Select(name => name.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var favoriteProductNames = (request.FavoriteIngredients ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            await using var transaction =
                await db.Database.BeginTransactionAsync();

            var newProfile = new UserProfile
            {
                UserId = user.Id
            };

            db.UserProfiles.Add(newProfile);
            await db.SaveChangesAsync();

            var savedDietNames = new List<string>();

            foreach (var dietName in dietNames)
            {
                var diet = await db.Diets
                    .FirstOrDefaultAsync(d =>
                        d.Name.ToLower() == dietName.ToLower());

                if (diet is null)
                {
                    diet = new Diet { Name = dietName };
                    db.Diets.Add(diet);
                    await db.SaveChangesAsync();
                }

                db.UserDiets.Add(new UserDiet
                {
                    UserId = user.Id,
                    DietId = diet.Id
                });

                savedDietNames.Add(diet.Name);
            }

            var savedFavoriteProductNames = new List<string>();

            foreach (var productName in favoriteProductNames)
            {
                var product = await db.Products
                    .FirstOrDefaultAsync(p =>
                        p.Name.ToLower() == productName.ToLower());

                if (product is null)
                {
                    product = new Product { Name = productName };
                    db.Products.Add(product);
                    await db.SaveChangesAsync();
                }

                db.UserFavoriteProducts.Add(new UserFavoriteProduct
                {
                    UserId = user.Id,
                    ProductId = product.Id
                });

                savedFavoriteProductNames.Add(product.Name);
            }

            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            var profileDto = new ProfileDto(
                newProfile.Id,
                (user.FirstName + " " + user.LastName).Trim(),
                user.Email,
                savedDietNames,
                string.Join(", ", savedFavoriteProductNames)
            );

            return TypedResults.Created(
                $"/api/profiles/{newProfile.Id}",
                profileDto);
        });
    }
}
