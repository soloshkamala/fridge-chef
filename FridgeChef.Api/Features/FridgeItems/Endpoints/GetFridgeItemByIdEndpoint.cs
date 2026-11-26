using FridgeChef.Api.Data;
using FridgeChef.Api.Features.FridgeItems.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FridgeChef.Api.Services;

namespace FridgeChef.Api.Features.FridgeItems.Endpoints;

public static class GetFridgeItemByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}", async Task<Results<Ok<FridgeItemResponse>, ProblemHttpResult>>
            (int id,
             HttpRequest request,
             FridgeChefDbContext dbContext,
             ClaimsPrincipal user,
             CurrentUserService currentUserService) =>
        {
            var userId = await currentUserService.GetUserIdAsync(user);

            if (userId == null)
            {
                return TypedResults.Problem(
                    statusCode: 401,
                    title: "User is not authenticated");
            }

            var isEnglish = CatalogLocalizationService.IsEnglish(request);

            var fridgeItem = await dbContext.FridgeItems
                .AsNoTracking()
                .Where(x => x.Id == id && x.UserId == userId.Value)
                .Include(x => x.Product)
                .Include(x => x.Unit)
                .FirstOrDefaultAsync();

            if (fridgeItem == null)
            {
                return TypedResults.Problem(
                    statusCode: 404,
                    title: "Product not found");
            }

            var item = new FridgeItemResponse
            {
                Id = fridgeItem.Id,
                Name = fridgeItem.Product.Name,
                DisplayName = CatalogLocalizationService.Translate(
                    fridgeItem.Product.Name,
                    fridgeItem.Product.NameEn,
                    isEnglish),
                Quantity = fridgeItem.Quantity,
                Unit = fridgeItem.Unit.ShortName,
                DisplayUnit = CatalogLocalizationService.Translate(
                    fridgeItem.Unit.ShortName,
                    fridgeItem.Unit.ShortNameEn,
                    isEnglish)
            };

            return TypedResults.Ok(item);
        })
        .ProducesProblem(401)
        .ProducesProblem(404);
    }
}
