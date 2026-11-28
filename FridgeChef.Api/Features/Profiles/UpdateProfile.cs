
using System.Diagnostics;
using System.Security.Claims;
using FridgeChef.Api.Data;
using FridgeChef.Api.Data.Entities;
using FridgeChef.Api.Services;
using FridgeChef.Api.Hubs;
using FridgeChef.Api.Observability;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FridgeChef.Api.Features.Profiles;

public static class UpdateProfile
{
    public static RouteHandlerBuilder MapUpdateProfile(
        this RouteGroupBuilder group)
    {
        return group.MapPut(
            "/{id:int}",
            async Task<Results<
                Ok<ProfileDto>,
                NotFound,
                ValidationProblem,
                Conflict<string>,
                UnauthorizedHttpResult>> (
                int id,
                UpdateProfileRequest request,
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

                using var activity =
                    FridgeTelemetry.ActivitySource.StartActivity(
                        "Profile.Update");

                activity?.SetTag("profile.operation", "update");
                activity?.SetTag("profile.user_id", userId.Value);
                activity?.SetTag("profile.id", id);

                logger.LogInformation(
                    "Updating profile {ProfileId} for user {UserId}",
                    id,
                    userId.Value);

                if (string.IsNullOrWhiteSpace(request.Name) ||
                    string.IsNullOrWhiteSpace(request.Email) ||
                    !System.Net.Mail.MailAddress.TryCreate(
                        request.Email, out var emailAddress) ||
                    emailAddress.Address != request.Email.Trim())
                {
                    return TypedResults.ValidationProblem(
                        new Dictionary<string, string[]>
                        {
                            ["Profile"] =
                                ["Вкажіть коректні ім'я та email."]
                        });
                }

                if (request.Diets is null ||
                    request.Diets.Any(string.IsNullOrWhiteSpace))
                {
                    return TypedResults.ValidationProblem(
                        new Dictionary<string, string[]>
                        {
                            ["Diets"] =
                                ["Назви дієт не можуть бути порожніми."]
                        });
                }

                var profile = await db.UserProfiles
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(
                        p => p.Id == id && p.UserId == userId.Value);

                if (profile is null)
                {
                    return TypedResults.NotFound();
                }

                var email = request.Email.Trim();

                if (!string.Equals(
                    profile.User.Email,
                    email,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return TypedResults.Conflict(
                        "Email профілю не збігається з email користувача.");
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

                var nameParts = request.Name.Trim()
                    .Split(
                        ' ',
                        2,
                        StringSplitOptions.RemoveEmptyEntries);

                profile.User.FirstName = nameParts[0];
                profile.User.LastName =
                    nameParts.Length > 1 ? nameParts[1] : "";

                var existingUserDiets = await db.UserDiets
                    .Where(ud => ud.UserId == userId.Value)
                    .ToListAsync();

                var existingFavoriteProducts =
                    await db.UserFavoriteProducts
                        .Where(ufp => ufp.UserId == userId.Value)
                        .ToListAsync();

                db.UserDiets.RemoveRange(existingUserDiets);
                db.UserFavoriteProducts.RemoveRange(
                    existingFavoriteProducts);

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
                        UserId = userId.Value,
                        DietId = diet.Id
                    });

                    savedDietNames.Add(diet.Name);
                }

                var savedFavoriteProductNames = new List<string>();

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
                            p => p.Name.ToLower() == productName.ToLower());

                        if (product is null)
                        {
                            await db.Database.ExecuteSqlInterpolatedAsync(
                                $"""
                                INSERT INTO "Products" ("Name", "CategoryId")
                                VALUES ({productName}, {category.Id})
                                ON CONFLICT ("Name") DO NOTHING
                                """);

                            product = await db.Products.FirstAsync(
                                p => p.Name.ToLower() == productName.ToLower());
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

                logger.LogInformation(
                    "Profile {ProfileId} updated successfully for user {UserId}",
                    profile.Id,
                    userId.Value);

               
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
                    profile.Id,
                    (profile.User.FirstName + " " +
                     profile.User.LastName).Trim(),
                    profile.User.Email,
                    savedDietNames,
                    string.Join(", ", savedFavoriteProductNames)
                );

                return TypedResults.Ok(profileDto);
            })
            .RequireAuthorization("ApiWrite");
    }
}
