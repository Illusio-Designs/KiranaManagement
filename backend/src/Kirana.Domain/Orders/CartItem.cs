using Kirana.Domain.Common;

namespace Kirana.Domain.Orders;

/// <summary>
/// A line in a marketplace cart. References a store + variant and snapshots the
/// price at add-time (MRP + selling price) so totals are stable in the cart.
/// </summary>
public class CartItem : BaseEntity
{
    public Guid CartId { get; set; }
    public Cart? Cart { get; set; }

    public Guid StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;

    public Guid ProductVariantId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string VariantName { get; set; } = string.Empty;

    public int Quantity { get; set; }
    public decimal UnitMrp { get; set; }
    public decimal UnitSellingPrice { get; set; }
}
