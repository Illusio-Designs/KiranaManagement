namespace Kirana.Application.Inventory;

public interface IInventoryService
{
    /// <summary>Applies a stock change to a variant and records a ledger entry.</summary>
    Task<StockLedgerEntryDto> AdjustStockAsync(Guid variantId, AdjustStockRequest request, CancellationToken ct = default);

    Task<IReadOnlyList<LowStockItemDto>> GetLowStockAsync(CancellationToken ct = default);

    Task<IReadOnlyList<StockLedgerEntryDto>> GetLedgerAsync(Guid variantId, CancellationToken ct = default);
}
