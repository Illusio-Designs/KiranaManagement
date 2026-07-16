using Kirana.Application.Carts;
using Kirana.Application.Common.Exceptions;
using Kirana.Application.Common.Interfaces;
using Kirana.Domain.Common.Enums;
using Kirana.Domain.Inventory;
using Kirana.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Kirana.Application.Orders;

public class CheckoutService : ICheckoutService
{
    private readonly IAppDbContext _db;
    private readonly DeliveryEstimateOptions _delivery;

    public CheckoutService(IAppDbContext db, IOptions<DeliveryEstimateOptions> delivery)
    {
        _db = db;
        _delivery = delivery.Value;
    }

    public async Task<OrderDto> CheckoutAsync(Guid customerId, CheckoutRequest request, CancellationToken ct = default)
    {
        var cart = await _db.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.CustomerId == customerId, ct);
        if (cart is null || cart.Items.Count == 0)
            throw new AppException("Your cart is empty.");

        var order = new Order
        {
            CustomerId = customerId,
            OrderNumber = GenerateOrderNumber(),
            ContactName = request.ContactName.Trim(),
            ContactPhone = request.ContactPhone.Trim(),
            AddressLine = request.AddressLine.Trim(),
            City = request.City?.Trim(),
            Pincode = request.Pincode?.Trim(),
            PaymentMode = request.PaymentMode,
            IsCod = request.PaymentMode == PaymentMode.Cash
        };
        // Online payment is mocked as captured; COD is collected on delivery.
        order.PaymentStatus = order.IsCod ? PaymentStatus.Pending : PaymentStatus.Paid;

        foreach (var group in cart.Items.GroupBy(i => i.StoreId))
        {
            var store = await _db.Stores.FirstOrDefaultAsync(s => s.Id == group.Key, ct)
                        ?? throw AppException.NotFound("A store in your cart was not found.");
            if (store.Status != StoreStatus.Active)
                throw new AppException($"'{store.Name}' is not currently accepting orders.");

            var storeOrder = new StoreOrder
            {
                StoreId = store.Id,
                StoreName = store.Name,
                Status = StoreOrderStatus.New
            };

            foreach (var item in group)
            {
                var variant = await _db.ProductVariants
                    .IgnoreQueryFilters()
                    .Include(v => v.Product)
                    .FirstOrDefaultAsync(v => v.Id == item.ProductVariantId, ct)
                    ?? throw AppException.NotFound("A product in your cart is no longer available.");

                if (!variant.IsActive)
                    throw new AppException($"'{variant.Name}' is no longer available.");
                if (variant.StockQuantity < item.Quantity)
                    throw new AppException($"'{variant.Product!.Name} — {variant.Name}' has only {variant.StockQuantity} left.");

                var unit = variant.SellingPrice;
                var lineTotal = unit * item.Quantity;

                storeOrder.Lines.Add(new OrderLine
                {
                    StoreId = store.Id,
                    ProductVariantId = variant.Id,
                    ProductName = variant.Product!.Name,
                    VariantName = variant.Name,
                    Quantity = item.Quantity,
                    UnitMrp = variant.Mrp,
                    UnitSellingPrice = unit,
                    LineDiscount = (variant.Mrp - unit) * item.Quantity,
                    LineTotal = lineTotal
                });

                variant.StockQuantity -= item.Quantity;
                _db.StockLedger.Add(new StockLedgerEntry
                {
                    StoreId = store.Id,
                    ProductVariantId = variant.Id,
                    ChangeQuantity = -item.Quantity,
                    BalanceAfter = variant.StockQuantity,
                    MovementType = StockMovementType.Sale,
                    Reason = $"Order {order.OrderNumber}"
                });
            }

            storeOrder.Subtotal = storeOrder.Lines.Sum(l => l.LineTotal);
            storeOrder.MrpTotal = storeOrder.Lines.Sum(l => l.UnitMrp * l.Quantity);
            storeOrder.Discount = storeOrder.MrpTotal - storeOrder.Subtotal;
            order.StoreOrders.Add(storeOrder);
        }

        order.ItemsSubtotal = order.StoreOrders.Sum(s => s.Subtotal);
        order.DiscountTotal = order.StoreOrders.Sum(s => s.Discount);
        order.DeliveryFee = EstimateDelivery(order.ItemsSubtotal, order.StoreOrders.Count);
        order.GrandTotal = order.ItemsSubtotal + order.DeliveryFee;

        _db.Orders.Add(order);
        _db.CartItems.RemoveRange(cart.Items);
        await _db.SaveChangesAsync(ct);

        return OrderMappings.Map(order);
    }

    public async Task<IReadOnlyList<OrderDto>> GetMyOrdersAsync(Guid customerId, CancellationToken ct = default)
    {
        var orders = await _db.Orders
            .IgnoreQueryFilters()
            .Where(o => o.CustomerId == customerId)
            .Include(o => o.StoreOrders).ThenInclude(s => s.Lines)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
        return orders.Select(OrderMappings.Map).ToList();
    }

    public async Task<OrderDto> GetMyOrderAsync(Guid customerId, Guid orderId, CancellationToken ct = default)
    {
        var order = await _db.Orders
            .IgnoreQueryFilters()
            .Include(o => o.StoreOrders).ThenInclude(s => s.Lines)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == customerId, ct)
            ?? throw AppException.NotFound("Order not found.");
        return OrderMappings.Map(order);
    }

    private static string GenerateOrderNumber()
        => $"ORD-{DateTime.UtcNow:yyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..4].ToUpperInvariant()}";

    private decimal EstimateDelivery(decimal itemsSubtotal, int storeCount)
    {
        if (storeCount == 0) return 0m;
        if (itemsSubtotal >= _delivery.FreeAboveOrderValue) return 0m;
        return _delivery.BaseFee + _delivery.PerAdditionalStoreFee * (storeCount - 1);
    }
}
