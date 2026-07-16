using Kirana.Domain.Common.Enums;

namespace Kirana.Application.Purchasing;

public record CreateSupplierRequest(string Name, string? Phone, string? Email, string? Gstin, string? AddressLine);
public record SupplierDto(Guid Id, string Name, string? Phone, string? Email, string? Gstin, string? AddressLine, bool IsActive);

public record CreatePurchaseOrderLineRequest(Guid ProductVariantId, int Quantity, decimal UnitCost);

public record CreatePurchaseOrderRequest(
    Guid SupplierId,
    string? Notes,
    IReadOnlyList<CreatePurchaseOrderLineRequest> Lines);

public record PurchaseOrderLineDto(
    Guid Id, Guid ProductVariantId, string VariantName, int Quantity, decimal UnitCost, decimal LineTotal);

public record PurchaseOrderDto(
    Guid Id,
    Guid SupplierId,
    string PoNumber,
    PurchaseStatus Status,
    decimal TotalCost,
    string? Notes,
    DateTime? ReceivedAt,
    DateTime CreatedAt,
    IReadOnlyList<PurchaseOrderLineDto> Lines);
