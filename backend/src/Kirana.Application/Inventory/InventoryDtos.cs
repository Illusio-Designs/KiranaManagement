using Kirana.Domain.Common.Enums;

namespace Kirana.Application.Inventory;

public record AdjustStockRequest(int ChangeQuantity, StockMovementType MovementType, string? Reason);

public record StockLedgerEntryDto(
    Guid Id, Guid ProductVariantId, int ChangeQuantity, int BalanceAfter,
    StockMovementType MovementType, string? Reason, DateTime CreatedAt);

public record LowStockItemDto(
    Guid VariantId, Guid ProductId, string ProductName, string VariantName,
    int StockQuantity, int ReorderLevel);
