using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.Profiles;

public static class GetProfile
{
    public static RouteHandlerBuilder MapGetProfile(
        this RouteGroupBuilder group)
    {
        return group.MapGet("/{id:int}",
            Results<Ok<ProfileDto>, NotFound> (
                int id,
                ProfileStore store) =>
            {
                var profile = store.GetById(id);

                return profile is null
                    ? TypedResults.NotFound()
                    : TypedResults.Ok(profile);
            });
    }
}