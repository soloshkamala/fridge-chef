using System.Security.Claims;
using FridgeChef.Api.Data;
using FridgeChef.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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
            ?? user.FindFirst(
                "http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

        var tid = user.FindFirst("tid")?.Value
            ?? user.FindFirst(
                "http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

        if (string.IsNullOrWhiteSpace(oid) ||
            string.IsNullOrWhiteSpace(tid))
            return null;

        var existingUser = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.EntraObjectId == oid &&
                x.EntraTenantId == tid);

        if (existingUser != null)
            return existingUser.Id;

        var email = user.FindFirst("preferred_username")?.Value
            ?? user.FindFirst("upn")?.Value
            ?? user.FindFirst(ClaimTypes.Upn)?.Value
            ?? user.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrWhiteSpace(email))
            return null;

        email = email.Trim().ToLowerInvariant();

        var emailExists = await _db.Users
            .AnyAsync(x => x.Email == email);

        if (emailExists)
            return null;

        var name = user.FindFirst("name")?.Value
            ?? user.Identity?.Name
            ?? "";

        var parts = name.Split(
            ' ',
            2,
            StringSplitOptions.RemoveEmptyEntries);

        var newUser = new User
        {
            Email = email,
            FirstName = parts.Length > 0 ? parts[0] : "",
            LastName = parts.Length > 1 ? parts[1] : "",
            EntraObjectId = oid,
            EntraTenantId = tid
        };

        _db.Users.Add(newUser);

        try
        {
            await _db.SaveChangesAsync();
            return newUser.Id;
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException pg &&
                  pg.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            _db.Entry(newUser).State = EntityState.Detached;

            return await _db.Users
                .AsNoTracking()
                .Where(x =>
                    x.EntraObjectId == oid &&
                    x.EntraTenantId == tid)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync();
        }
    }
}
