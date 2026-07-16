using Kirana.Domain.Common;

namespace Kirana.Domain.Catalog;

public class Category : BaseEntity, ITenantEntity
{
    public Guid StoreId { get; set; }

    public string Name { get; set; } = string.Empty;
    public Guid? ParentCategoryId { get; set; }
    public bool IsActive { get; set; } = true;
}
