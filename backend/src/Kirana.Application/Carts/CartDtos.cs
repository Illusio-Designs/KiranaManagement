namespace Kirana.Application.Carts;

public record AddCartItemRequest(Guid StoreId, Guid ProductVariantId, int Quantity);
public record UpdateCartItemRequest(int Quantity);

public record CartItemDto(
    Guid Id,
    Guid StoreId,
    string StoreName,
    Guid ProductVariantId,
    string ProductName,
    string VariantName,
    int Quantity,
    decimal UnitMrp,
    decimal UnitSellingPrice,
    decimal LineTotal,
    decimal LineDiscount);

public record CartStoreGroupDto(
    Guid StoreId,
    string StoreName,
    IReadOnlyList<CartItemDto> Items,
    decimal Subtotal,
    decimal MrpTotal,
    decimal Discount);

public record CartSummaryDto(
    Guid CartId,
    IReadOnlyList<CartStoreGroupDto> Stores,
    int StoreCount,
    int ItemCount,
    decimal ItemsSubtotal,
    decimal TotalMrp,
    decimal TotalDiscount,
    decimal EstimatedDeliveryFee,
    decimal GrandTotal);
