namespace Kirana.Application.Common.Interfaces;

/// <summary>
/// The store (tenant) the current request belongs to, resolved from the JWT
/// <c>store_id</c> claim. Used by AppDbContext to filter every tenant entity.
/// </summary>
public interface ICurrentTenant
{
    /// <summary>Current store id, or <see cref="Guid.Empty"/> when there is no tenant
    /// (unauthenticated, or the platform SuperAdmin).</summary>
    Guid StoreId { get; }

    bool HasTenant { get; }
}
