
using System.Security.Claims;
using FridgeChef.Api.Data;
using FridgeChef.Api.Data.Entities;
using FridgeChef.Api.Services;
using FridgeChef.Api.Hubs;
using FridgeChef.Api.Observability;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FridgeChef.Api.Features.Profiles;

public static class CreateProfile
{
    public static RouteHandlerBuilder MapCreateProfile(
        this RouteGroupBuilder group)
    {
        return group.MapPost(
            "/",
            async Task<Results<
                Created<ProfileDto>,
                ValidationProblem,
                Conflict<string>,
                UnauthorizedHttpResult>> (
                CreateProfileRequest request,
                ClaimsPrincipal user,
                FridgeChefDbContext db,
                CurrentUserService currentUser,
                IHubContext<ProfileHub> profileHub,
                ILogger<Program> logger) =>
            {
                var userId = await currentUser.GetUserIdAsync(user);

                if (userId is null)
                {
                    return TypedResults.Unauthorized();
                }

                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return TypedResults.ValidationProblem(
                        new Dictionary<string, string[]>
                        {
                            ["Name"] = ["Ім'я є обов'язковим."]
                        });
                }

                if (string.IsNullOrWhiteSpace(request.Email) ||
                    !System.Net.Mail.MailAddress.TryCreate(
                        request.Email, out var emailAddress) ||
                    emailAddress.Address != request.Email.Trim())
                {
                    return TypedResults.ValidationProblem(
                        new Dictionary<string, string[]>
                        {
                            ["Email"] = ["Вкажіть коректний email."]
                        });
                }

                if (request.Diets is null ||
                    request.Diets.Any(string.IsNullOrWhiteSpace))
                {
                    return TypedResults.ValidationProblem(
                        new Dictionary<string, string[]>
                        {
                            ["Diets"] = [
                                "Назви дієт не можуть бути порожніми."
                            ]
                        });
                }

                var dbUser = await db.Users.FirstOrDefaultAsync(
                    u => u.Id == userId.Value);

                if (dbUser is null)
                {
                    return TypedResults.Unauthorized();
                }

                if (!string.Equals(
                    dbUser.Email,
                    request.Email.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return TypedResults.Conflict(
                        "Email профілю не збігається з email користувача.");
                }

                if (await db.UserProfiles.AnyAsync(
                    p => p.UserId == userId.Value))
                {
                    return TypedResults.Conflict(
                        "Профіль для цього користувача вже існує.");
                }

                var dietNames = request.Diets
                    .Select(name => name.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var favoriteProductNames =
                    (request.FavoriteIngredients ?? "")
                    .Split(
                        ',',
                        StringSplitOptions.RemoveEmptyEntries |
                        StringSplitOptions.TrimEntries)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                await using var transaction =
                    await db.Database.BeginTransactionAsync();

                try
                {
                    var newProfile = new UserProfile
                    {
                        UserId = userId.Value
                    };

                    db.UserProfiles.Add(newProfile);
                    await db.SaveChangesAsync();

                    var savedDietNames = new List<string>();

                    foreach (var dietName in dietNames)
                    {
                        var diet = await db.Diets.FirstOrDefaultAsync(
                            d => d.Name.ToLower() == dietName.ToLower());

                        if (diet is null)
                        {
                            diet = new Diet { Name = dietName };
                            db.Diets.Add(diet);
                            await db.SaveChangesAsync();
                        }

                        db.UserDiets.Add(new UserDiet
                        {
                            UserId = userId.Value,
                            DietId = diet.Id
                        });

                        savedDietNames.Add(diet.Name);
                    }

                    var savedFavoriteProductNames =
                        new List<string>();

                    if (favoriteProductNames.Count > 0)
                    {
                        const string defaultCategoryName = "Інгредієнти";

                        await db.Database.ExecuteSqlInterpolatedAsync(
                            $"""
                            INSERT INTO "Categories" ("Name")
                            VALUES ({defaultCategoryName})
                            ON CONFLICT ("Name") DO NOTHING
                            """);

                        var category = await db.Categories.FirstAsync(
                            c => c.Name == defaultCategoryName);

                        foreach (var productName in favoriteProductNames)
                        {
                            var product = await db.Products.FirstOrDefaultAsync(
                                p => p.Name.ToLower() ==
                                     productName.ToLower());

                            if (product is null)
                            {
                                await db.Database.ExecuteSqlInterpolatedAsync(
                                    $"""
                                    INSERT INTO "Products" ("Name", "CategoryId")
                                    VALUES ({productName}, {category.Id})
                                    ON CONFLICT ("Name") DO NOTHING
                                    """);

                                product = await db.Products.FirstAsync(
                                    p => p.Name.ToLower() ==
                                         productName.ToLower());
                            }

                            db.UserFavoriteProducts.Add(
                                new UserFavoriteProduct
                                {
                                    UserId = userId.Value,
                                    ProductId = product.Id
                                });

                            savedFavoriteProductNames.Add(product.Name);
                        }
                    }

                    await db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // Повідомляємо лише власника профілю.
                    try
                    {
                        await profileHub.Clients
                            .Group(ProfileHubGroups.ForUser(userId.Value))
                            .SendAsync("ProfileChanged");
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(
                            ex,
                            "Failed to send ProfileChanged notification for user {UserId}",
                            userId.Value);
                    }

                    var profileDto = new ProfileDto(
                        newProfile.Id,
                        (dbUser.FirstName + " " + dbUser.LastName).Trim(),
                        dbUser.Email,
                        savedDietNames,
                        string.Join(", ", savedFavoriteProductNames)
                    );

                    return TypedResults.Created(
                        $"/api/profiles/{newProfile.Id}",
                        profileDto);
                }
                catch (DbUpdateException)
                {
                    await transaction.RollbackAsync();

                    var profileExists = await db.UserProfiles
                        .AsNoTracking()
                        .AnyAsync(p => p.UserId == userId.Value);

                    if (profileExists)
                    {
                        return TypedResults.Conflict(
                            "Профіль для цього користувача вже існує.");
                    }

                    throw;
                }
            })
            .RequireAuthorization("ApiWrite");
    }
}
