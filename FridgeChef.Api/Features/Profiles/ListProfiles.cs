
using FridgeChef.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace FridgeChef.Api.Features.Profiles;

public static class ListProfiles
{
    public static RouteHandlerBuilder MapListProfiles(
        this RouteGroupBuilder group)
    {
        return group.MapGet("/", async (FridgeChefDbContext db) =>
        {
            var profiles = await db.UserProfiles
                .AsNoTracking()
                .Select(p => new
                {
                    p.Id,
                    p.UserId,
                    Name = (p.User.FirstName + " " + p.User.LastName).Trim(),
                    p.User.Email,

                    Diets = db.UserDiets
                        .Where(ud => ud.UserId == p.UserId)
                        .Select(ud => ud.Diet.Name)
                        .ToList(),

                    FavoriteProducts = db.UserFavoriteProducts
                        .Where(ufp => ufp.UserId == p.UserId)
                        .OrderBy(ufp => ufp.ProductId)
                        .Select(ufp => ufp.Product.Name)
                        .ToList()
                })
                .ToListAsync();

            var result = profiles
                .Select(p => new ProfileDto(
                    p.Id,
                    p.Name,
                    p.Email,
                    p.Diets,
                    string.Join(", ", p.FavoriteProducts)
                ))
                .ToList();

            return TypedResults.Ok(result);
        });
    }
}
