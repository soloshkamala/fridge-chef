
using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.Users;

public static class CreateUser
{
    public static Results<
        Created<UserResponse>,
        ProblemHttpResult> Handle(
        CreateUserRequest request,
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
            Email = request.Email.Trim().ToLowerInvariant(),
            DisplayName = request.DisplayName.Trim()
        };

        user.PasswordHash = passwords.Hash(
            user, request.Password);

        if (!store.TryAdd(user, out var created))
            return TypedResults.Problem(
                statusCode: 409,
                title: "Email already exists");

        var response = new UserResponse(
            created!.Id,
            created.Email,
            created.DisplayName);

        return TypedResults.Created(
            $"/api/users/{created.Id}",
            response);
    }
}
