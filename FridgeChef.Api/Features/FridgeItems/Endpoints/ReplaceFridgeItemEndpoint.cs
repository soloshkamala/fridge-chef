
using FridgeChef.Api.Data;
using FridgeChef.Api.Features.FridgeItems.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FridgeChef.Api.Services;
using FridgeChef.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using FridgeChef.Api.Observability;

namespace FridgeChef.Api.Features.FridgeItems.Endpoints;

public static class ReplaceFridgeItemEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{id:int}", async Task<Results<NoContent, ProblemHttpResult>>
            (int id, SaveFridgeItemRequest request, FridgeChefDbContext dbContext, ClaimsPrincipal user, CurrentUserService currentUserService,
            IHubContext<FridgeChefHub> hubContext, ILogger<FridgeChefHub> logger) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Unit) ||
                request.Quantity <= 0)
            {
                return TypedResults.Problem(
                    statusCode: 400,
                    title: "Invalid product data");
            }

            var userId = await currentUserService.GetUserIdAsync(user);

            if (userId == null)
            {
                return TypedResults.Problem(
                    statusCode: 401,
                    title: "User is not authenticated");
            }

            var item = await dbContext.FridgeItems
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (item == null)
            {
                return TypedResults.Problem(
                    statusCode: 404,
                    title: "Product not found");
            }

            var productName = request.Name.Trim();
            var unitName = request.Unit.Trim();

            var product = await dbContext.Products
                .FirstOrDefaultAsync(x => x.Name.ToLower() == productName.ToLower());

            if (product == null)
            {
                return TypedResults.Problem(
                    statusCode: 400,
                    title: "Product not found in catalog");
            }

            var unit = await dbContext.Units
                .FirstOrDefaultAsync(x => x.ShortName == unitName);

            if (unit == null)
            {
                return TypedResults.Problem(
                    statusCode: 400,
                    title: "Unit not found");
            }

            var isUnitAllowed = await dbContext.ProductUnits
                .AnyAsync(x => x.ProductId == product.Id && x.UnitId == unit.Id);

            if (!isUnitAllowed)
            {
                return TypedResults.Problem(
                    statusCode: 400,
                    title: "Unit is not allowed for this product");
            }

            var duplicateExists = await dbContext.FridgeItems.AnyAsync(x =>
                x.Id != id &&
                x.UserId == userId &&
                x.ProductId == product.Id &&
                x.UnitId == unit.Id);

            if (duplicateExists)
            {
                return TypedResults.Problem(
                    statusCode: 409,
                    title: "Product already exists");
            }

            item.ProductId = product.Id;
            item.UnitId = unit.Id;
            item.Quantity = request.Quantity;

            await dbContext.SaveChangesAsync();

            using (var activity = FridgeTelemetry.ActivitySource.StartActivity("FridgeItemChanged"))
            {
                activity?.SetTag("fridge.item.id", item.Id);
                activity?.SetTag("fridge.operation", "update");

                await hubContext.Clients.Groups(
                    FridgeHubGroups.ForItem(userId.Value, item.Id),
                    FridgeHubGroups.ForUser(userId.Value))
                    .SendAsync("FridgeItemChanged", item.Id);
            }

            FridgeSignalRLogger.FridgeItemChanged(
                logger, item.Id, userId.Value);

            return TypedResults.NoContent();
        })
        .RequireAuthorization("ApiWrite")
        .ProducesProblem(400)
        .ProducesProblem(401)
        .ProducesProblem(404)
        .ProducesProblem(409);
    }
}
