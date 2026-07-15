using Kirana.Domain.Common;
using Kirana.Domain.Common.Enums;

namespace Kirana.Domain.Identity;

/// <summary>
/// A login account. Deliberately NOT an ITenantEntity: login looks a user up by
/// email before any tenant context exists, and the SuperAdmin has no store.
/// A store's staff carry that store's <see cref="StoreId"/>.
/// </summary>
public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    /// <summary>Null for the platform SuperAdmin; set for store staff.</summary>
    public Guid? StoreId { get; set; }

    public bool IsActive { get; set; } = true;
}
