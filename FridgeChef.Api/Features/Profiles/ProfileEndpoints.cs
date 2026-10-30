namespace FridgeChef.Api.Features.Profiles;

public static class ProfileEndpoints
{
    public static RouteGroupBuilder MapProfileEndpoints(
        this RouteGroupBuilder group)
    {
        group.MapListProfiles();
        group.MapGetProfile();
        group.MapCreateProfile();
        group.MapUpdateProfile();
        group.MapDeleteProfile();

        return group;
    }
}