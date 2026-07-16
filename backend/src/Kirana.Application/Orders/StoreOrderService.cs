using Kirana.Application.Common.Exceptions;
using Kirana.Application.Common.Interfaces;
using Kirana.Domain.Common.Enums;
using Kirana.Domain.Inventory;
using Kirana.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace Kirana.Application.Orders;

public class StoreOrderService : IStoreOrderService
{
    private readonly IAppDbContext _db;

    public StoreOrderService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<StoreOrderDto>> GetStoreOrdersAsync(StoreOrderStatus? status, CancellationToken ct = default)
    {
        var query = _db.StoreOrders.Include(s => s.Lines).AsQueryable();
        if (status is StoreOrderStatus s)
            query = query.Where(so => so.Status == s);

        var orders = await query.OrderByDescending(so => so.CreatedAt).ToListAsync(ct);
        return orders.Select(OrderMappings.Map).ToList();
    }

    public async Task<StoreOrderDto> GetStoreOrderAsync(Guid storeOrderId, CancellationToken ct = default)
        => OrderMappings.Map(await LoadAsync(storeOrderId, ct));

    public async Task<StoreOrderDto> AcceptAsync(Guid storeOrderId, CancellationToken ct = default)
        => await TransitionAsync(storeOrderId, StoreOrderStatus.New, StoreOrderStatus.Accepted, ct);

    public async Task<StoreOrderDto> PackAsync(Guid storeOrderId, CancellationToken ct = default)
        => await TransitionAsync(storeOrderId, StoreOrderStatus.Accepted, StoreOrderStatus.Packed, ct);

    public async Task<StoreOrderDto> MarkReadyAsync(Guid storeOrderId, CancellationToken ct = default)
        => await TransitionAsync(storeOrderId, StoreOrderStatus.Packed, StoreOrderStatus.Ready, ct);

    public async Task<StoreOrderDto> CancelAsync(Guid storeOrderId, CancellationToken ct = default)
    {
        var so = await LoadAsync(storeOrderId, ct);
        if (so.Status is StoreOrderStatus.Ready or StoreOrderStatus.Cancelled)
            throw new AppException($"A {so.Status} order part cannot be cancelled.");

        // Restock the cancelled part.
        foreach (var line in so.Lines)
        {
            var variant = await _db.ProductVariants.FirstOrDefaultAsync(v => v.Id == line.ProductVariantId, ct);
            if (variant is null) continue;
            variant.StockQuantity += line.Quantity;
            _db.StockLedger.Add(new StockLedgerEntry
            {
                StoreId = so.StoreId,
                ProductVariantId = variant.Id,
                ChangeQuantity = line.Quantity,
                BalanceAfter = variant.StockQuantity,
                MovementType = StockMovementType.ReturnIn,
                Reason = $"Cancelled store order {so.Id}"
            });
        }

        so.Status = StoreOrderStatus.Cancelled;
        await RecomputeParentAndSaveAsync(so.OrderId, ct);
        return OrderMappings.Map(so);
    }

    private async Task<StoreOrderDto> TransitionAsync(
        Guid storeOrderId, StoreOrderStatus from, StoreOrderStatus to, CancellationToken ct)
    {
        var so = await LoadAsync(storeOrderId, ct);
        if (so.Status != from)
            throw new AppException($"Cannot move an order part from {so.Status} to {to}.");

        so.Status = to;
        await RecomputeParentAndSaveAsync(so.OrderId, ct);
        return OrderMappings.Map(so);
    }

    private async Task<StoreOrder> LoadAsync(Guid storeOrderId, CancellationToken ct)
        => await _db.StoreOrders.Include(s => s.Lines).FirstOrDefaultAsync(s => s.Id == storeOrderId, ct)
           ?? throw AppException.NotFound("Order part not found.");

    /// <summary>Recomputes the parent order status from all its store parts, then saves.</summary>
    private async Task RecomputeParentAndSaveAsync(Guid orderId, CancellationToken ct)
    {
        // Cross-store view of the parent is needed, so bypass the tenant filter.
        var order = await _db.Orders
            .IgnoreQueryFilters()
            .Include(o => o.StoreOrders)
            .FirstOrDefaultAsync(o => o.Id == orderId, ct);

        if (order is not null && order.Status is not (OrderStatus.OutForDelivery or OrderStatus.Delivered))
        {
            var active = order.StoreOrders.Where(s => s.Status != StoreOrderStatus.Cancelled).ToList();
            order.Status = active.Count == 0
                ? OrderStatus.Cancelled
                : active.All(s => s.Status == StoreOrderStatus.Ready)
                    ? OrderStatus.ReadyForPickup
                    : active.Any(s => s.Status is StoreOrderStatus.Accepted or StoreOrderStatus.Packed or StoreOrderStatus.Ready)
                        ? OrderStatus.Preparing
                        : OrderStatus.Placed;
        }

        await _db.SaveChangesAsync(ct);
    }
}
