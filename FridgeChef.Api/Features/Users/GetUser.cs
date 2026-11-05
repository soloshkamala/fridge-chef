
using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.Users;

public static class GetUser
{
    public static Results<Ok<UserResponse>, ProblemHttpResult> Handle(
        int id, UserStore store)
    {
        var user = store.GetById(id);

        if (user is null)
            return TypedResults.Problem(
                statusCode: 404,
                title: "User not found");

        return TypedResults.Ok(
            new UserResponse(
                user.Id, user.Email, user.DisplayName));
    }
}
