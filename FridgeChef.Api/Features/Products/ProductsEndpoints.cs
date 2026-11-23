
using FridgeChef.Api.Data;
using FridgeChef.Api.Features.Products.Models;
using Microsoft.EntityFrameworkCore;

namespace FridgeChef.Api.Features.Products;

public static class ProductsEndpoints
{
    public static RouteGroupBuilder MapProductsEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").RequireAuthorization("ApiRead")
            .WithTags("Products");

        group.MapGet("/", async (FridgeChefDbContext db) =>
        {
            var products = await db.Products
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Select(p => new ProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    AllowedUnits = db.ProductUnits
                        .Where(pu => pu.ProductId == p.Id)
                        .OrderBy(pu => pu.UnitId)
                        .Select(pu => pu.Unit.ShortName)
                        .ToList()
                })
                .ToListAsync();

            return Results.Ok(products);
        });

        return group;
    }
}
