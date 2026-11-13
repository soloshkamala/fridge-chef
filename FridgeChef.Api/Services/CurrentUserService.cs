using System.Security.Claims;
using FridgeChef.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace FridgeChef.Api.Services;

public class CurrentUserService
{
    private readonly FridgeChefDbContext _db;

    public CurrentUserService(FridgeChefDbContext db)
    {
        _db = db;
    }

    public async Task<int?> GetUserIdAsync(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
            return null;

        var oid = user.FindFirst("oid")?.Value
            ?? user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

        var tid = user.FindFirst("tid")?.Value
            ?? user.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

        if (string.IsNullOrWhiteSpace(oid) ||
            string.IsNullOrWhiteSpace(tid))
            return null;

        return await _db.Users
            .AsNoTracking()
            .Where(x => x.EntraObjectId == oid &&
                        x.EntraTenantId == tid)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync();
    }
}
