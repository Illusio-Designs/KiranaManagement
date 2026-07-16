using Kirana.Application.Common.Exceptions;
using Kirana.Application.Common.Interfaces;
using Kirana.Domain.Common.Enums;
using Kirana.Domain.Inventory;
using Kirana.Domain.Purchasing;
using Microsoft.EntityFrameworkCore;

namespace Kirana.Application.Purchasing;

public class PurchaseService : IPurchaseService
{
    private readonly IAppDbContext _db;
    private readonly ICurrentTenant _tenant;

    public PurchaseService(IAppDbContext db, ICurrentTenant tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    private Guid StoreId => _tenant.HasTenant
        ? _tenant.StoreId
        : throw AppException.Unauthorized("No store context.");

    // ---- Suppliers ----
    public async Task<SupplierDto> CreateSupplierAsync(CreateSupplierRequest request, CancellationToken ct = default)
    {
        var supplier = new Supplier
        {
            StoreId = StoreId,
            Name = request.Name.Trim(),
            Phone = request.Phone?.Trim(),
            Email = request.Email?.Trim(),
            Gstin = request.Gstin?.Trim().ToUpperInvariant(),
            AddressLine = request.AddressLine?.Trim()
        };
        _db.Suppliers.Add(supplier);
        await _db.SaveChangesAsync(ct);
        return MapSupplier(supplier);
    }

    public async Task<IReadOnlyList<SupplierDto>> GetSuppliersAsync(CancellationToken ct = default)
    {
        var suppliers = await _db.Suppliers.OrderBy(s => s.Name).ToListAsync(ct);
        return suppliers.Select(MapSupplier).ToList();
    }

    // ---- Purchase orders ----
    public async Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderRequest request, CancellationToken ct = default)
    {
        if (request.Lines is null || request.Lines.Count == 0)
            throw new AppException("A purchase order needs at least one line.");

        var storeId = StoreId;

        var supplierExists = await _db.Suppliers.AnyAsync(s => s.Id == request.SupplierId, ct);
        if (!supplierExists) throw AppException.NotFound("Supplier not found.");

        var po = new PurchaseOrder
        {
            StoreId = storeId,
            SupplierId = request.SupplierId,
            PoNumber = GeneratePoNumber(),
            Status = PurchaseStatus.Draft,
            Notes = request.Notes?.Trim()
        };

        foreach (var line in request.Lines)
        {
            if (line.Quantity <= 0) throw new AppException("Line quantity must be at least 1.");
            if (line.UnitCost < 0) throw new AppException("Unit cost cannot be negative.");

            var variant = await _db.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.Id == line.ProductVariantId, ct)
                ?? throw AppException.NotFound($"Variant {line.ProductVariantId} not found.");

            po.Lines.Add(new PurchaseOrderLine
            {
                StoreId = storeId,
                ProductVariantId = variant.Id,
                VariantName = $"{variant.Product!.Name} — {variant.Name}",
                Quantity = line.Quantity,
                UnitCost = line.UnitCost,
                LineTotal = line.UnitCost * line.Quantity
            });
        }

        po.TotalCost = po.Lines.Sum(l => l.LineTotal);
        _db.PurchaseOrders.Add(po);
        await _db.SaveChangesAsync(ct);

        return MapOrder(po);
    }

    public async Task<IReadOnlyList<PurchaseOrderDto>> GetPurchaseOrdersAsync(CancellationToken ct = default)
    {
        var orders = await _db.PurchaseOrders
            .Include(p => p.Lines)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);
        return orders.Select(MapOrder).ToList();
    }

    public async Task<PurchaseOrderDto> GetPurchaseOrderAsync(Guid id, CancellationToken ct = default)
    {
        var po = await _db.PurchaseOrders.Include(p => p.Lines).FirstOrDefaultAsync(p => p.Id == id, ct)
                 ?? throw AppException.NotFound("Purchase order not found.");
        return MapOrder(po);
    }

    public async Task<PurchaseOrderDto> ReceivePurchaseOrderAsync(Guid id, CancellationToken ct = default)
    {
        var po = await _db.PurchaseOrders.Include(p => p.Lines).FirstOrDefaultAsync(p => p.Id == id, ct)
                 ?? throw AppException.NotFound("Purchase order not found.");

        if (po.Status != PurchaseStatus.Draft)
            throw new AppException($"Only draft purchase orders can be received (current: {po.Status}).");

        foreach (var line in po.Lines)
        {
            var variant = await _db.ProductVariants.FirstOrDefaultAsync(v => v.Id == line.ProductVariantId, ct);
            if (variant is null) continue; // variant deleted since PO was raised

            variant.StockQuantity += line.Quantity;
            _db.StockLedger.Add(new StockLedgerEntry
            {
                StoreId = po.StoreId,
                ProductVariantId = variant.Id,
                ChangeQuantity = line.Quantity,
                BalanceAfter = variant.StockQuantity,
                MovementType = StockMovementType.PurchaseReceipt,
                Reason = $"GRN {po.PoNumber}"
            });
        }

        po.Status = PurchaseStatus.Received;
        po.ReceivedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return MapOrder(po);
    }

    private static string GeneratePoNumber()
        => $"PO-{DateTime.UtcNow:yyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..4].ToUpperInvariant()}";

    private static SupplierDto MapSupplier(Supplier s) =>
        new(s.Id, s.Name, s.Phone, s.Email, s.Gstin, s.AddressLine, s.IsActive);

    private static PurchaseOrderDto MapOrder(PurchaseOrder p) => new(
        p.Id, p.SupplierId, p.PoNumber, p.Status, p.TotalCost, p.Notes, p.ReceivedAt, p.CreatedAt,
        p.Lines.Select(l => new PurchaseOrderLineDto(
            l.Id, l.ProductVariantId, l.VariantName, l.Quantity, l.UnitCost, l.LineTotal)).ToList());
}
