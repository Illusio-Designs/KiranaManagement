using Kirana.Domain.Common;

namespace Kirana.Domain.Catalog;

/// <summary>
/// A catalog product owned by one store. Implements <see cref="ITenantEntity"/>,
/// so it is automatically filtered by the current tenant — included in Phase 0
/// to prove multi-tenant isolation end-to-end. Full catalog comes in Phase 1.
/// </summary>
public class Product : BaseEntity, ITenantEntity
{
    public Guid StoreId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Sku { get; set; }
    public decimal Price { get; set; }
    public decimal TaxRate { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;
}
