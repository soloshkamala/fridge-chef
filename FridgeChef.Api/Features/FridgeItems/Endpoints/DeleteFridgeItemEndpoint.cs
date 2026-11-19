using FridgeChef.Api.Data;
using FridgeChef.Api.Hubs;
using FridgeChef.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FridgeChef.Api.Observability;


namespace FridgeChef.Api.Features.FridgeItems.Endpoints;

public static class DeleteFridgeItemEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:int}", async Task<Results<NoContent, ProblemHttpResult>>
            (int id, FridgeChefDbContext dbContext,ClaimsPrincipal user, CurrentUserService currentUserService, 
            IHubContext<FridgeChefHub> hubContext, ILogger<FridgeChefHub> logger) =>
        {
            var userId = await currentUserService.GetUserIdAsync(user);

            if (userId == null)
            {
                return TypedResults.Problem(
                    statusCode: 401,
                    title: "User is not authenticated");
            }

            var item = await dbContext.FridgeItems
            .FirstOrDefaultAsync(x =>
                x.Id == id && x.UserId == userId.Value);

            if (item == null)
            {
                return TypedResults.Problem(
                    statusCode: 404,
                    title: "Product not found");
            }

            dbContext.FridgeItems.Remove(item);
            await dbContext.SaveChangesAsync();

            using (var activity = FridgeTelemetry.ActivitySource.StartActivity("FridgeItemChanged"))
            {
                activity?.SetTag("fridge.item.id", item.Id);
                activity?.SetTag("fridge.operation", "delete");

                await hubContext.Clients.Groups(
                    FridgeHubGroups.ForItem(userId.Value, item.Id),
                    FridgeHubGroups.ForUser(userId.Value))
                    .SendAsync("FridgeItemChanged", item.Id);
            }

            FridgeSignalRLogger.FridgeItemChanged(
                logger, item.Id, userId.Value);

            return TypedResults.NoContent();
        })
        .ProducesProblem(401)
        .ProducesProblem(404);
    }
}