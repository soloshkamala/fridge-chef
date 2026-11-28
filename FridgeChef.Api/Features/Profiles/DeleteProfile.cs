
using System.Security.Claims;
using FridgeChef.Api.Data;
using FridgeChef.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FridgeChef.Api.Features.Profiles;

public static class DeleteProfile
{
    public static RouteHandlerBuilder MapDeleteProfile(
        this RouteGroupBuilder group)
    {
        return group.MapDelete("/{id:int}",
            async Task<Results<NoContent, NotFound, UnauthorizedHttpResult>> (
                int id,
                ClaimsPrincipal user,
                FridgeChefDbContext db,
                CurrentUserService currentUser) =>
            {
                var userId = await currentUser.GetUserIdAsync(user);

                if (userId is null)
                {
                    return TypedResults.Unauthorized();
                }

                var profile = await db.UserProfiles
                    .FirstOrDefaultAsync(
                        p => p.Id == id && p.UserId == userId.Value);

                if (profile is null)
                {
                    return TypedResults.NotFound();
                }

                await using var transaction =
                    await db.Database.BeginTransactionAsync();

                var userDiets = await db.UserDiets
                    .Where(ud => ud.UserId == userId.Value)
                    .ToListAsync();

                db.UserDiets.RemoveRange(userDiets);

                var favoriteProducts = await db.UserFavoriteProducts
                    .Where(ufp => ufp.UserId == userId.Value)
                    .ToListAsync();

                db.UserFavoriteProducts.RemoveRange(favoriteProducts);

                db.UserProfiles.Remove(profile);

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return TypedResults.NoContent();
            })
            .RequireAuthorization("ApiWrite");
    }
}
