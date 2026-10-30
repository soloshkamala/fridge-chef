namespace FridgeChef.Api.Features.Profiles;

public static class ListProfiles
{
    public static RouteHandlerBuilder MapListProfiles(
        this RouteGroupBuilder group)
    {
        return group.MapGet("/", (ProfileStore store) =>
            TypedResults.Ok(store.GetAll()));
    }
}