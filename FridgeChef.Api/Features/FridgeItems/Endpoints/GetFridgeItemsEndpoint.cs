using FridgeChef.Api.Data;
using FridgeChef.Api.Features.FridgeItems.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FridgeChef.Api.Services;

namespace FridgeChef.Api.Features.FridgeItems.Endpoints;

public static class GetFridgeItemsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async Task<Results<Ok<List<FridgeItemResponse>>, ProblemHttpResult>>
            (HttpRequest request,
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

            var fridgeItems = await dbContext.FridgeItems
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Include(x => x.Product)
                .Include(x => x.Unit)
                .ToListAsync();

            var items = fridgeItems.Select(x => new FridgeItemResponse
            {
                Id = x.Id,
                Name = x.Product.Name,
                DisplayName = CatalogLocalizationService.Translate(
                    x.Product.Name, x.Product.NameEn, isEnglish),
                Quantity = x.Quantity,
                Unit = x.Unit.ShortName,
                DisplayUnit = CatalogLocalizationService.Translate(
                    x.Unit.ShortName, x.Unit.ShortNameEn, isEnglish)
            }).ToList();

            return TypedResults.Ok(items);
        })
        .ProducesProblem(401);
    }
}
