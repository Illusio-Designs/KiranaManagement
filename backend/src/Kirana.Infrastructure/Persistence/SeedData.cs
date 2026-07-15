using Kirana.Application.Common.Interfaces;
using Kirana.Domain.Common.Enums;
using Kirana.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kirana.Infrastructure.Persistence;

/// <summary>Seeds the platform SuperAdmin so the approval workflow is usable on a fresh DB.</summary>
public static class SeedData
{
    public static async Task SeedAsync(
        AppDbContext db, IPasswordHasher hasher, string adminEmail, string adminPassword,
        CancellationToken ct = default)
    {
        adminEmail = adminEmail.Trim().ToLowerInvariant();

        var exists = await db.Users.AnyAsync(u => u.Email == adminEmail, ct);
        if (exists) return;

        db.Users.Add(new User
        {
            Email = adminEmail,
            FullName = "Platform Super Admin",
            PasswordHash = hasher.Hash(adminPassword),
            Role = UserRole.SuperAdmin,
            StoreId = null,
            IsActive = true
        });

        await db.SaveChangesAsync(ct);
    }
}
