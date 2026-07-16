using Kirana.Domain.Common.Enums;
using Kirana.Domain.Orders;

namespace Kirana.Application.Orders;

public record OrderLineDto(
    Guid Id, Guid ProductVariantId, string ProductName, string VariantName,
    int Quantity, decimal UnitMrp, decimal UnitSellingPrice, decimal LineDiscount, decimal LineTotal);

public record StoreOrderDto(
    Guid Id, Guid OrderId, Guid StoreId, string StoreName, StoreOrderStatus Status,
    decimal Subtotal, decimal MrpTotal, decimal Discount, DateTime CreatedAt,
    IReadOnlyList<OrderLineDto> Lines);

public record OrderDto(
    Guid Id,
    string OrderNumber,
    Guid CustomerId,
    string ContactName,
    string ContactPhone,
    string AddressLine,
    string? City,
    string? Pincode,
    PaymentMode PaymentMode,
    PaymentStatus PaymentStatus,
    bool IsCod,
    decimal ItemsSubtotal,
    decimal DiscountTotal,
    decimal DeliveryFee,
    decimal GrandTotal,
    OrderStatus Status,
    DateTime CreatedAt,
    IReadOnlyList<StoreOrderDto> StoreOrders);

/// <summary>Entity → DTO mappers shared by checkout and store-order services.</summary>
public static class OrderMappings
{
    public static OrderLineDto Map(OrderLine l) => new(
        l.Id, l.ProductVariantId, l.ProductName, l.VariantName,
        l.Quantity, l.UnitMrp, l.UnitSellingPrice, l.LineDiscount, l.LineTotal);

    public static StoreOrderDto Map(StoreOrder s) => new(
        s.Id, s.OrderId, s.StoreId, s.StoreName, s.Status,
        s.Subtotal, s.MrpTotal, s.Discount, s.CreatedAt,
        s.Lines.Select(Map).ToList());

    public static OrderDto Map(Order o) => new(
        o.Id, o.OrderNumber, o.CustomerId, o.ContactName, o.ContactPhone,
        o.AddressLine, o.City, o.Pincode, o.PaymentMode, o.PaymentStatus, o.IsCod,
        o.ItemsSubtotal, o.DiscountTotal, o.DeliveryFee, o.GrandTotal, o.Status, o.CreatedAt,
        o.StoreOrders.OrderBy(s => s.StoreName).Select(Map).ToList());
}
