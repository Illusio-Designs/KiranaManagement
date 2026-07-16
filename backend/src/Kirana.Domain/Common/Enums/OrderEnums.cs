namespace Kirana.Domain.Common.Enums;

/// <summary>Status of the parent marketplace order (derived from its store parts + delivery).</summary>
public enum OrderStatus
{
    Placed = 0,
    Preparing = 1,
    ReadyForPickup = 2,
    OutForDelivery = 3,
    Delivered = 4,
    Cancelled = 5
}

/// <summary>Status of one store's part of an order (what the store fulfils).</summary>
public enum StoreOrderStatus
{
    New = 0,
    Accepted = 1,
    Packed = 2,
    Ready = 3,
    Cancelled = 4
}

public enum PaymentStatus
{
    Pending = 0,   // e.g. COD not yet collected
    Paid = 1,
    Failed = 2,
    Refunded = 3
}
