using Kirana.Domain.Common;

namespace Kirana.Domain.Orders;

public class OrderLine : BaseEntity, ITenantEntity
{
    public Guid StoreId { get; set; }
    public Guid StoreOrderId { get; set; }
    public StoreOrder? StoreOrder { get; set; }

    public Guid ProductVariantId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string VariantName { get; set; } = string.Empty;

    public int Quantity { get; set; }
    public decimal UnitMrp { get; set; }
    public decimal UnitSellingPrice { get; set; }
    public decimal LineDiscount { get; set; }
    public decimal LineTotal { get; set; }
}
