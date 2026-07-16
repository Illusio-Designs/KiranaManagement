namespace Kirana.Application.Purchasing;

public interface IPurchaseService
{
    Task<SupplierDto> CreateSupplierAsync(CreateSupplierRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<SupplierDto>> GetSuppliersAsync(CancellationToken ct = default);

    Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<PurchaseOrderDto>> GetPurchaseOrdersAsync(CancellationToken ct = default);
    Task<PurchaseOrderDto> GetPurchaseOrderAsync(Guid id, CancellationToken ct = default);

    /// <summary>Goods receipt: marks the PO received and increases variant stock.</summary>
    Task<PurchaseOrderDto> ReceivePurchaseOrderAsync(Guid id, CancellationToken ct = default);
}
