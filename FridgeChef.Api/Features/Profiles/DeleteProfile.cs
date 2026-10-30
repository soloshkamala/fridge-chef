using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.Profiles;

public static class DeleteProfile
{
    public static RouteHandlerBuilder MapDeleteProfile(
        this RouteGroupBuilder group)
    {
        return group.MapDelete("/{id:int}",
            Results<NoContent, NotFound> (
                int id,
                ProfileStore store) =>
            {
                return store.Delete(id)
                    ? TypedResults.NoContent()
                    : TypedResults.NotFound();
            });
    }
}