using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.Profiles;

public static class UpdateProfile
{
    public static RouteHandlerBuilder MapUpdateProfile(
        this RouteGroupBuilder group)
    {
        return group.MapPut("/{id:int}",
            Results<Ok<ProfileDto>, NotFound, ValidationProblem, Conflict<string>> (
                int id,
                UpdateProfileRequest request,
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



               var result = store.TryUpdate(id, request);

if (result == "NotFound")
{
    return TypedResults.NotFound();
}

if (result == "Conflict")
{
    return TypedResults.Conflict(
        "Профіль із таким email уже існує.");
}

return TypedResults.Ok(store.GetById(id)!);
            });
    }
}