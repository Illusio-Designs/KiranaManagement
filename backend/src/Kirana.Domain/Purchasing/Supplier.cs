using Kirana.Domain.Common;

namespace Kirana.Domain.Purchasing;

public class Supplier : BaseEntity, ITenantEntity
{
    public Guid StoreId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Gstin { get; set; }
    public string? AddressLine { get; set; }
    public bool IsActive { get; set; } = true;
}
