using Kirana.Application.Common.Exceptions;
using Kirana.Application.Common.Interfaces;
using Kirana.Domain.Inventory;
using Microsoft.EntityFrameworkCore;

namespace Kirana.Application.Inventory;

public class InventoryService : IInventoryService
{
    private readonly IAppDbContext _db;

    public InventoryService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<StockLedgerEntryDto> AdjustStockAsync(
        Guid variantId, AdjustStockRequest request, CancellationToken ct = default)
    {
        if (request.ChangeQuantity == 0)
            throw new AppException("Change quantity cannot be zero.");

        var variant = await _db.ProductVariants.FirstOrDefaultAsync(v => v.Id == variantId, ct)
                      ?? throw AppException.NotFound("Variant not found.");

        var newBalance = variant.StockQuantity + request.ChangeQuantity;
        if (newBalance < 0)
            throw new AppException($"Insufficient stock: on hand {variant.StockQuantity}, change {request.ChangeQuantity}.");

        variant.StockQuantity = newBalance;

        var entry = new StockLedgerEntry
        {
            StoreId = variant.StoreId,
            ProductVariantId = variant.Id,
            ChangeQuantity = request.ChangeQuantity,
            BalanceAfter = newBalance,
            MovementType = request.MovementType,
            Reason = request.Reason?.Trim()
        };
        _db.StockLedger.Add(entry);
        await _db.SaveChangesAsync(ct);

        return Map(entry);
    }

    public async Task<IReadOnlyList<LowStockItemDto>> GetLowStockAsync(CancellationToken ct = default)
    {
        return await _db.ProductVariants
            .Where(v => v.IsActive && v.StockQuantity <= v.ReorderLevel)
            .Include(v => v.Product)
            .OrderBy(v => v.StockQuantity)
            .Select(v => new LowStockItemDto(
                v.Id, v.ProductId, v.Product!.Name, v.Name, v.StockQuantity, v.ReorderLevel))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<StockLedgerEntryDto>> GetLedgerAsync(Guid variantId, CancellationToken ct = default)
    {
        var entries = await _db.StockLedger
            .Where(e => e.ProductVariantId == variantId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(ct);
        return entries.Select(Map).ToList();
    }

    private static StockLedgerEntryDto Map(StockLedgerEntry e) => new(
        e.Id, e.ProductVariantId, e.ChangeQuantity, e.BalanceAfter, e.MovementType, e.Reason, e.CreatedAt);
}
