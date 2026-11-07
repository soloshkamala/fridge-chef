
using FridgeChef.Api.Data;
using FridgeChef.Api.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FridgeChef.Api.Features.ShoppingItems;

public static class ShoppingItemEndpoints
{
    public static RouteGroupBuilder MapShoppingItemEndpoints(
        this RouteGroupBuilder group)
    {
        group.RequireAuthorization();

        group.MapGet("/", async (
            FridgeChefDbContext db,
            HttpContext context) =>
        {
            var user = await GetOrCreateUserAsync(db, context);

            var items = await db.ShoppingItems
                .AsNoTracking()
                .Where(x => x.UserId == user.Id)
                .Include(x => x.Product)
                    .ThenInclude(x => x.Category)
                .Include(x => x.Unit)
                .Select(x => new ShoppingItemResponse(
                    x.Id,
                    x.Product.Name,
                    (double)x.Quantity,
                    x.Unit.ShortName,
                    x.Product.Category.Name,
                    x.IsPurchased,
                    x.IsInFridge))
                .ToListAsync();

            return Results.Ok(items);
        });

        group.MapGet("/{id:int}", async (
            int id,
            FridgeChefDbContext db,
            HttpContext context) =>
        {
            var user = await GetOrCreateUserAsync(db, context);

            var item = await db.ShoppingItems
                .AsNoTracking()
                .Where(x => x.Id == id && x.UserId == user.Id)
                .Include(x => x.Product)
                    .ThenInclude(x => x.Category)
                .Include(x => x.Unit)
                .Select(x => new ShoppingItemResponse(
                    x.Id,
                    x.Product.Name,
                    (double)x.Quantity,
                    x.Unit.ShortName,
                    x.Product.Category.Name,
                    x.IsPurchased,
                    x.IsInFridge))
                .FirstOrDefaultAsync();

            return item is null
                ? Results.NotFound()
                : Results.Ok(item);
        });

        group.MapPost("/", async (
            [FromBody] CreateShoppingItemRequest request,
            FridgeChefDbContext db,
            HttpContext context) =>
        {
            var error = Validate(
                request.Name,
                request.Quantity,
                request.Unit,
                request.Category);

            if (error is not null)
                return Results.ValidationProblem(error);

            var user = await GetOrCreateUserAsync(db, context);

            var productName = request.Name.Trim();
            var unitName = request.Unit.Trim();
            var categoryName = request.Category.Trim();

            var category = await db.Categories
                .FirstOrDefaultAsync(x => x.Name == categoryName);

            if (category is null)
            {
                category = new Category { Name = categoryName };
                db.Categories.Add(category);
                await db.SaveChangesAsync();
            }

            var product = await db.Products
                .FirstOrDefaultAsync(x => x.Name == productName);

            if (product is null)
            {
                product = new Product
                {
                    Name = productName,
                    CategoryId = category.Id
                };

                db.Products.Add(product);
                await db.SaveChangesAsync();
            }

            var unit = await db.Units
                .FirstOrDefaultAsync(x =>
                    x.ShortName == unitName || x.Name == unitName);

            if (unit is null)
            {
                unit = new Unit
                {
                    Name = unitName,
                    ShortName = unitName
                };

                db.Units.Add(unit);
                await db.SaveChangesAsync();
            }

            var item = new ShoppingItem
            {
                UserId = user.Id,
                ProductId = product.Id,
                UnitId = unit.Id,
                Quantity = (decimal)request.Quantity,
                IsPurchased = false,
                IsInFridge = false
            };

            db.ShoppingItems.Add(item);
            await db.SaveChangesAsync();

            var response = new ShoppingItemResponse(
                item.Id,
                product.Name,
                (double)item.Quantity,
                unit.ShortName,
                category.Name,
                item.IsPurchased,
                item.IsInFridge);

            return Results.Created(
                $"/api/shopping-items/{item.Id}",
                response);
        });

        group.MapPut("/{id:int}", async (
            int id,
            [FromBody] UpdateShoppingItemRequest request,
            FridgeChefDbContext db,
            HttpContext context) =>
        {
            var error = Validate(
                request.Name,
                request.Quantity,
                request.Unit,
                request.Category);

            if (error is not null)
                return Results.ValidationProblem(error);

            var user = await GetOrCreateUserAsync(db, context);

            var item = await db.ShoppingItems
                .Include(x => x.Product)
                    .ThenInclude(x => x.Category)
                .Include(x => x.Unit)
                .FirstOrDefaultAsync(x =>
                    x.Id == id && x.UserId == user.Id);

            if (item is null)
                return Results.NotFound();

            var categoryName = request.Category.Trim();
            var productName = request.Name.Trim();
            var unitName = request.Unit.Trim();

            var category = await db.Categories
                .FirstOrDefaultAsync(x => x.Name == categoryName);

            if (category is null)
            {
                category = new Category { Name = categoryName };
                db.Categories.Add(category);
                await db.SaveChangesAsync();
            }

            var product = await db.Products
                .FirstOrDefaultAsync(x => x.Name == productName);

            if (product is null)
            {
                product = new Product
                {
                    Name = productName,
                    CategoryId = category.Id
                };
                db.Products.Add(product);
                await db.SaveChangesAsync();
            }
            else
            {
                product.CategoryId = category.Id;
            }

            var unit = await db.Units
                .FirstOrDefaultAsync(x =>
                    x.ShortName == unitName || x.Name == unitName);

            if (unit is null)
            {
                unit = new Unit
                {
                    Name = unitName,
                    ShortName = unitName
                };
                db.Units.Add(unit);
                await db.SaveChangesAsync();
            }

            item.ProductId = product.Id;
            item.UnitId = unit.Id;
            item.Quantity = (decimal)request.Quantity;
            item.IsPurchased = request.IsPurchased;
            item.IsInFridge = request.IsInFridge;

            await db.SaveChangesAsync();

            return Results.Ok();
        });

        group.MapDelete("/{id:int}", async (
            int id,
            FridgeChefDbContext db,
            HttpContext context) =>
        {
            var user = await GetOrCreateUserAsync(db, context);

            var item = await db.ShoppingItems
                .FirstOrDefaultAsync(x =>
                    x.Id == id && x.UserId == user.Id);

            if (item is null)
                return Results.NotFound();

            db.ShoppingItems.Remove(item);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        return group;
    }

    private static async Task<User> GetOrCreateUserAsync(
        FridgeChefDbContext db,
        HttpContext context)
    {
        var email = context.User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidOperationException(
                "Authenticated user email is missing.");

        email = email.Trim().ToLowerInvariant();

        var user = await db.Users
            .FirstOrDefaultAsync(x => x.Email == email);

        if (user is not null)
            return user;

        var displayName = context.User.FindFirstValue(ClaimTypes.Name)
            ?? "";

        var parts = displayName.Split(
            ' ',
            2,
            StringSplitOptions.RemoveEmptyEntries);

        user = new User
        {
            Email = email,
            FirstName = parts.Length > 0 ? parts[0] : "",
            LastName = parts.Length > 1 ? parts[1] : ""
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return user;
    }

    private static Dictionary<string, string[]>? Validate(
        string name,
        double quantity,
        string unit,
        string category)
    {
        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(unit) ||
            string.IsNullOrWhiteSpace(category) ||
            name.Trim().Length > 100 ||
            unit.Trim().Length > 30 ||
            category.Trim().Length > 50 ||
            quantity < 0.01 ||
            quantity > 1000 ||
            double.IsNaN(quantity) ||
            double.IsInfinity(quantity))
        {
            return new Dictionary<string, string[]>
            {
                ["request"] =
                [
                    "Перевірте назву, кількість, одиницю та категорію."
                ]
            };
        }

        return null;
    }
}
