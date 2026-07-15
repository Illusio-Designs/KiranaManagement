namespace Kirana.Domain.Common;

/// <summary>
/// Marks a row as owned by a single store (tenant). Every entity implementing
/// this is automatically filtered by the current tenant in AppDbContext, so a
/// store can only ever read/write its own rows.
/// </summary>
public interface ITenantEntity
{
    Guid StoreId { get; set; }
}
