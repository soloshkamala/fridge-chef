
using System.Security.Claims;
using FridgeChef.Api.Data;
using FridgeChef.Api.Data.Entities;
using FridgeChef.Api.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FridgeChef.Api.Features.Profiles;

public static class GetMyProfile
{
    public static RouteHandlerBuilder MapGetMyProfile(
        this RouteGroupBuilder group)
    {
        return group.MapGet("/me", async (
            ClaimsPrincipal user,
            FridgeChefDbContext db,
            CurrentUserService currentUser) =>
        {
            var oid = user.FindFirst("oid")?.Value
                ?? user.FindFirst(
                    "http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            var tid = user.FindFirst("tid")?.Value
                ?? user.FindFirst(
                    "http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

            if (string.IsNullOrWhiteSpace(oid) ||
                string.IsNullOrWhiteSpace(tid))
            {
                return Results.Unauthorized();
            }

            var email = user.FindFirst("preferred_username")?.Value
                ?? user.FindFirst("upn")?.Value
                ?? user.FindFirst(ClaimTypes.Upn)?.Value
                ?? user.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
            {
                return Results.Problem(
                    "Microsoft account email is missing.",
                    statusCode: 400);
            }

            email = email.Trim().ToLowerInvariant();

            var name = user.FindFirst("name")?.Value
                ?? user.Identity?.Name
                ?? "";

            var dbUser = await db.Users.FirstOrDefaultAsync(
                x => x.EntraObjectId == oid &&
                     x.EntraTenantId == tid);

            if (dbUser is null)
            {
                var emailExists = await db.Users.AnyAsync(
                    x => x.Email == email);

                if (emailExists)
                {
                    return Results.Conflict(
                        "An account with this email already exists.");
                }

                var parts = name.Split(
                    ' ',
                    2,
                    StringSplitOptions.RemoveEmptyEntries);

                dbUser = new User
                {
                    Email = email,
                    FirstName = parts.Length > 0 ? parts[0] : "",
                    LastName = parts.Length > 1 ? parts[1] : "",
                    EntraObjectId = oid,
                    EntraTenantId = tid
                };

                db.Users.Add(dbUser);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                    when (ex.InnerException is PostgresException pg &&
                          pg.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                    db.Entry(dbUser).State = EntityState.Detached;

                    dbUser = await db.Users.FirstOrDefaultAsync(
                        x => x.EntraObjectId == oid &&
                             x.EntraTenantId == tid);

                    if (dbUser is null)
                    {
                        return Results.Conflict(
                            "An account with this email already exists.");
                    }
                }
            }

            var currentUserId =
                await currentUser.GetUserIdAsync(user);

            if (currentUserId is null)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(new
            {
                UserId = currentUserId.Value,
                Name = name,
                Email = dbUser.Email
            });
        })
        .RequireAuthorization("ApiRead");
    }
}
