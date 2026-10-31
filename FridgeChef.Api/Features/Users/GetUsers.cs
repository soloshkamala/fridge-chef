
using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.Users;

public static class GetUsers
{
    public static Ok<List<UserResponse>> Handle(UserStore store)
    {
        var users = store.GetAll()
            .Select(x => new UserResponse(
                x.Id, x.Email, x.DisplayName))
            .ToList();

        return TypedResults.Ok(users);
    }
}
