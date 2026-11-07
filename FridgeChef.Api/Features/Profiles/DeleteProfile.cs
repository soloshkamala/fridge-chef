
using FridgeChef.Api.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FridgeChef.Api.Features.Profiles;

public static class DeleteProfile
{
    public static RouteHandlerBuilder MapDeleteProfile(
        this RouteGroupBuilder group)
    {
        return group.MapDelete("/{id:int}",
            async Task<Results<NoContent, NotFound>> (
                int id,
                FridgeChefDbContext db) =>
            {
                var profile = await db.UserProfiles
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (profile is null)
                {
                    return TypedResults.NotFound();
                }

                await using var transaction =
                    await db.Database.BeginTransactionAsync();

                
                var userDiets = await db.UserDiets
                    .Where(ud => ud.UserId == profile.UserId)
                    .ToListAsync();

                db.UserDiets.RemoveRange(userDiets);

                
                var favoriteProducts = await db.UserFavoriteProducts
                    .Where(ufp => ufp.UserId == profile.UserId)
                    .ToListAsync();

                db.UserFavoriteProducts.RemoveRange(favoriteProducts);

               
                db.UserProfiles.Remove(profile);

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return TypedResults.NoContent();
            });
    }
}
