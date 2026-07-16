using Kirana.Domain.Common;

namespace Kirana.Domain.Catalog;

/// <summary>
/// A catalog product owned by one store. Prices and stock live on its
/// <see cref="ProductVariant"/>s (e.g. 500 g / 1 kg packs), each with its own
/// MRP and selling price. Tenant-scoped (auto-filtered by store).
/// </summary>
public class Product : BaseEntity, ITenantEntity
{
    public Guid StoreId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Brand { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
}
