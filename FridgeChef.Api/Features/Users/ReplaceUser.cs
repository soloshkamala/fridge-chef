
using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.Users;

public static class ReplaceUser
{
    public static Results<
        NoContent,
        ProblemHttpResult> Handle(
        int id,
        ReplaceUserRequest request,
        UserStore store,
        UserPasswordService passwords)
    {
        var error = UserValidation.Validate(
            request.Email,
            request.Password,
            request.DisplayName);

        if (error is not null)
            return TypedResults.Problem(
                statusCode: 400,
                title: "Validation failed",
                detail: error);

        var user = new User
        {
            Id = id,
            Email = request.Email.Trim().ToLowerInvariant(),
            DisplayName = request.DisplayName.Trim()
        };

        user.PasswordHash = passwords.Hash(
            user, request.Password);

        var result = store.TryReplace(user);

        return result switch
        {
            UserUpdateResult.NotFound =>
                TypedResults.Problem(
                    statusCode: 404,
                    title: "User not found"),

            UserUpdateResult.Conflict =>
                TypedResults.Problem(
                    statusCode: 409,
                    title: "Email already exists"),

            _ => TypedResults.NoContent()
        };
    }
}
