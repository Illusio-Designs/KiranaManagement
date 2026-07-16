using Kirana.Domain.Common;
using Kirana.Domain.Common.Enums;

namespace Kirana.Domain.Orders;

/// <summary>
/// A marketplace order — the single order number the customer sees. Platform-level
/// (spans stores). It is split into per-store <see cref="StoreOrder"/> parts; the
/// customer pays once and gets one delivery (PRD §1.1.1, §5.7).
/// </summary>
public class Order : BaseEntity
{
    public Guid CustomerId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;

    // Delivery contact + address
    public string ContactName { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Pincode { get; set; }

    // Payment
    public PaymentMode PaymentMode { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public bool IsCod { get; set; }

    // Money
    public decimal ItemsSubtotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal GrandTotal { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Placed;

    public ICollection<StoreOrder> StoreOrders { get; set; } = new List<StoreOrder>();
}
