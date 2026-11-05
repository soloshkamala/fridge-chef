using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.Profiles;

public static class CreateProfile
{
    public static RouteHandlerBuilder MapCreateProfile(
        this RouteGroupBuilder group)
    {
        return group.MapPost("/",
            Results<Created<ProfileDto>, ValidationProblem, Conflict<string>> (
                CreateProfileRequest request,
                ProfileStore store) =>
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



                var profile = store.TryAdd(request);
                if (profile is null)
{
    return TypedResults.Conflict(
        "Профіль із таким email уже існує.");
}
                return TypedResults.Created(
                    $"/api/profiles/{profile.Id}", profile);
            });
    }
}