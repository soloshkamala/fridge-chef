
using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.Users;

public static class DeleteUser
{
    public static Results<NoContent, ProblemHttpResult> Handle(
        int id, UserStore store)
    {
        if (!store.Delete(id))
            return TypedResults.Problem(
                statusCode: 404,
                title: "User not found");

        return TypedResults.NoContent();
    }
}
