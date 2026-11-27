
using System.Security.Claims;
using FridgeChef.Api.Data;
using FridgeChef.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FridgeChef.Api.Features.Profiles;

public static class GetProfile
{
    public static RouteHandlerBuilder MapGetProfile(
        this RouteGroupBuilder group)
    {
        return group.MapGet("/{id:int}",
            async Task<Results<Ok<ProfileDto>, NotFound, UnauthorizedHttpResult>> (
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
                    .AsNoTracking()
                    .Where(p => p.Id == id && p.UserId == userId.Value)
                    .Select(p => new ProfileDto(
                        p.Id,
                        (p.User.FirstName + " " + p.User.LastName).Trim(),
                        p.User.Email,
                        db.UserDiets
                            .Where(ud => ud.UserId == p.UserId)
                            .Select(ud => ud.Diet.Name)
                            .ToList(),
                        string.Join(", ",
                            db.UserFavoriteProducts
                                .Where(ufp => ufp.UserId == p.UserId)
                                .OrderBy(ufp => ufp.ProductId)
                                .Select(ufp => ufp.Product.Name)
                                .ToList())
                    ))
                    .FirstOrDefaultAsync();

                return profile is null
                    ? TypedResults.NotFound()
                    : TypedResults.Ok(profile);
            })
            .RequireAuthorization("ApiRead");
    }
}
