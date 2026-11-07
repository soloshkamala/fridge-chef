
using FridgeChef.Api.Data;
using FridgeChef.Api.Data.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FridgeChef.Api.Features.Profiles;

public static class UpdateProfile
{
    public static RouteHandlerBuilder MapUpdateProfile(
        this RouteGroupBuilder group)
    {
        return group.MapPut("/{id:int}",
            async Task<Results<Ok<ProfileDto>, NotFound,
                ValidationProblem, Conflict<string>>> (
                int id,
                UpdateProfileRequest request,
                FridgeChefDbContext db) =>
            {
                if (string.IsNullOrWhiteSpace(request.Name) ||
                    string.IsNullOrWhiteSpace(request.Email) ||
                    !System.Net.Mail.MailAddress.TryCreate(
                        request.Email, out var emailAddress) ||
                    emailAddress.Address != request.Email)
                {
                    return TypedResults.ValidationProblem(
                        new Dictionary<string, string[]>
                        {
                            ["Profile"] = ["Вкажіть коректні ім'я та email."]
                        });
                }

                if (request.Diets is null ||
                    request.Diets.Any(string.IsNullOrWhiteSpace))
                {
                    return TypedResults.ValidationProblem(
                        new Dictionary<string, string[]>
                        {
                            ["Diets"] = ["Назви дієт не можуть бути порожніми."]
                        });
                }

                var profile = await db.UserProfiles
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == id);

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
                    .Where(ud => ud.UserId == profile.UserId)
                    .ToListAsync();

                
                var existingFavoriteProducts =
                    await db.UserFavoriteProducts
                        .Where(ufp => ufp.UserId == profile.UserId)
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
                        diet = new Diet
                        {
                            Name = dietName
                        };

                        db.Diets.Add(diet);
                        await db.SaveChangesAsync();
                    }

                    db.UserDiets.Add(new UserDiet
                    {
                        UserId = profile.UserId,
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
                        product = new Product
                        {
                            Name = productName
                        };

                        db.Products.Add(product);
                        await db.SaveChangesAsync();
                    }

                    db.UserFavoriteProducts.Add(
                        new UserFavoriteProduct
                        {
                            UserId = profile.UserId,
                            ProductId = product.Id
                        });

                    savedFavoriteProductNames.Add(product.Name);
                }

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                var profileDto = new ProfileDto(
                    profile.Id,
                    (profile.User.FirstName + " " +
                     profile.User.LastName).Trim(),
                    profile.User.Email,
                    savedDietNames,
                    string.Join(", ", savedFavoriteProductNames)
                );

                return TypedResults.Ok(profileDto);
            });
    }
}
