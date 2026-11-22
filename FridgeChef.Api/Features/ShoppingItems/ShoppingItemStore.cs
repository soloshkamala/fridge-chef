
using FridgeChef.Api.Data;
using FridgeChef.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FridgeChef.Api.Features.ShoppingItems;

public class ShoppingItemStore
{
    private readonly FridgeChefDbContext _db;

    public ShoppingItemStore(FridgeChefDbContext db)
    {
        _db = db;
    }

    public async Task<List<ShoppingItemResponse>> GetAllAsync(int userId)
    {
        var items = await _db.ShoppingItems
            .AsNoTracking()
            .Include(x => x.Product)
                .ThenInclude(p => p.Category)
            .Include(x => x.Unit)
            .Where(x => x.UserId == userId)
            .ToListAsync();

        return items.Select(MapToResponse).ToList();
    }

    public async Task<ShoppingItemResponse?> GetByIdAsync(int id, int userId)
    {
        var item = await _db.ShoppingItems
            .AsNoTracking()
            .Include(x => x.Product)
                .ThenInclude(p => p.Category)
            .Include(x => x.Unit)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        return item == null ? null : MapToResponse(item);
    }

    public async Task<ShoppingItemResponse?> AddAsync(
        CreateShoppingItemRequest request,
        int userId)
    {
        var product = await FindProductAsync(
            request.Name,
            request.Category);

        if (product == null)
            return null;

        var unit = await FindUnitAsync(request.Unit);

        if (unit == null)
            return null;

        var isAllowed = await _db.ProductUnits.AnyAsync(
            x => x.ProductId == product.Id && x.UnitId == unit.Id);

        if (!isAllowed)
            return null;

        var item = new ShoppingItem
        {
            UserId = userId,
            ProductId = product.Id,
            UnitId = unit.Id,
            Quantity = Convert.ToDecimal(request.Quantity),
            IsPurchased = false,
            IsInFridge = false
        };

        _db.ShoppingItems.Add(item);
        await _db.SaveChangesAsync();

        await _db.Entry(item).Reference(x => x.Product).LoadAsync();
        await _db.Entry(item.Product).Reference(x => x.Category).LoadAsync();
        await _db.Entry(item).Reference(x => x.Unit).LoadAsync();

        return MapToResponse(item);
    }

    public async Task<ShoppingItemResponse?> UpdateAsync(
        int id,
        UpdateShoppingItemRequest request,
        int userId)
    {
        var item = await _db.ShoppingItems
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (item == null)
            return null;

        var product = await FindProductAsync(
            request.Name,
            request.Category);

        if (product == null)
            return null;

        var unit = await FindUnitAsync(request.Unit);

        if (unit == null)
            return null;

        var isAllowed = await _db.ProductUnits.AnyAsync(
            x => x.ProductId == product.Id && x.UnitId == unit.Id);

        if (!isAllowed)
            return null;

        item.ProductId = product.Id;
        item.UnitId = unit.Id;
        item.Quantity = Convert.ToDecimal(request.Quantity);
        item.IsPurchased = request.IsPurchased;
        item.IsInFridge = request.IsInFridge;

        await _db.SaveChangesAsync();

        await _db.Entry(item).Reference(x => x.Product).LoadAsync();
        await _db.Entry(item.Product).Reference(x => x.Category).LoadAsync();
        await _db.Entry(item).Reference(x => x.Unit).LoadAsync();

        return MapToResponse(item);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var item = await _db.ShoppingItems
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (item == null)
            return false;

        _db.ShoppingItems.Remove(item);
        await _db.SaveChangesAsync();

        return true;
    }

    private async Task<Product?> FindProductAsync(
        string productName,
        string categoryName)
    {
        var name = productName.Trim().ToLower();
        var category = categoryName.Trim().ToLower();

        return await _db.Products
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x =>
                x.Name.ToLower() == name &&
                x.Category.Name.ToLower() == category);
    }

    private async Task<Unit?> FindUnitAsync(string unitName)
    {
        var name = unitName.Trim().ToLower();

        return await _db.Units.FirstOrDefaultAsync(x =>
            x.ShortName.ToLower() == name ||
            x.Name.ToLower() == name);
    }

    private static ShoppingItemResponse MapToResponse(ShoppingItem item)
    {
        return new ShoppingItemResponse(
            item.Id,
            item.Product.Name,
            (double)item.Quantity,
            item.Unit.ShortName,
            item.Product.Category.Name,
            item.IsPurchased,
            item.IsInFridge);
    }
}
