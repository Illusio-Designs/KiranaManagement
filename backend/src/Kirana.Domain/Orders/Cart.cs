using Kirana.Domain.Common;

namespace Kirana.Domain.Orders;

/// <summary>
/// A marketplace shopping cart for one customer. Platform-level (NOT tenant-
/// scoped): a single cart can hold items from multiple stores (PRD §1.1.1).
/// </summary>
public class Cart : BaseEntity
{
    public Guid CustomerId { get; set; }
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
