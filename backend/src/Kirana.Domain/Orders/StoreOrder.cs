using Kirana.Domain.Common;
using Kirana.Domain.Common.Enums;

namespace Kirana.Domain.Orders;

/// <summary>
/// One store's part of a marketplace <see cref="Order"/> — the unit that store
/// fulfils and books. Tenant-scoped, so a store sees only its own part.
/// </summary>
public class StoreOrder : BaseEntity, ITenantEntity
{
    public Guid StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;

    public Guid OrderId { get; set; }
    public Order? Order { get; set; }

    public StoreOrderStatus Status { get; set; } = StoreOrderStatus.New;

    public decimal Subtotal { get; set; }
    public decimal MrpTotal { get; set; }
    public decimal Discount { get; set; }

    public ICollection<OrderLine> Lines { get; set; } = new List<OrderLine>();
}
