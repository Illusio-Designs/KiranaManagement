using Kirana.Domain.Common;

namespace Kirana.Domain.Purchasing;

public class PurchaseOrderLine : BaseEntity, ITenantEntity
{
    public Guid StoreId { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }

    public Guid ProductVariantId { get; set; }
    public string VariantName { get; set; } = string.Empty;

    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal LineTotal { get; set; }
}
