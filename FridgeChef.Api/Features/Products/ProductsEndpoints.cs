using FridgeChef.Api.Data;
using FridgeChef.Api.Features.Products.Models;
using FridgeChef.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace FridgeChef.Api.Features.Products;

public static class ProductsEndpoints
{
    public static RouteGroupBuilder MapProductsEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .RequireAuthorization("ApiRead")
            .WithTags("Products");

        group.MapGet("/", async (
            HttpRequest request,
            FridgeChefDbContext db) =>
        {
            var isEnglish = CatalogLocalizationService.IsEnglish(request);

            var products = await db.Products
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.NameEn,
                    p.CategoryId,
                    CategoryName = p.Category.Name,
                    CategoryNameEn = p.Category.NameEn,
                    Units = db.ProductUnits
                        .Where(pu => pu.ProductId == p.Id)
                        .OrderBy(pu => pu.UnitId)
                        .Select(pu => new
                        {
                            pu.Unit.ShortName,
                            pu.Unit.ShortNameEn
                        })
                        .ToList()
                })
                .ToListAsync();

            var result = products.Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                DisplayName = CatalogLocalizationService.Translate(
                    p.Name, p.NameEn, isEnglish),
                CategoryId = p.CategoryId,
                CategoryName = p.CategoryName,
                DisplayCategoryName = CatalogLocalizationService.Translate(
                    p.CategoryName, p.CategoryNameEn, isEnglish),
                AllowedUnits = p.Units
                    .Select(u => u.ShortName)
                    .ToList(),
                DisplayUnits = p.Units.ToDictionary(
                    u => u.ShortName,
                    u => CatalogLocalizationService.Translate(
                        u.ShortName, u.ShortNameEn, isEnglish))
            }).ToList();

            return Results.Ok(result);
        });

        return group;
    }
}
