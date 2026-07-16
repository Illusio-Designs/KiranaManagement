using Kirana.Application.Common.Exceptions;
using Kirana.Application.Common.Interfaces;
using Kirana.Domain.Common.Enums;
using Kirana.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Kirana.Application.Carts;

public class CartService : ICartService
{
    private readonly IAppDbContext _db;
    private readonly DeliveryEstimateOptions _delivery;

    public CartService(IAppDbContext db, IOptions<DeliveryEstimateOptions> delivery)
    {
        _db = db;
        _delivery = delivery.Value;
    }

    public async Task<CartSummaryDto> GetSummaryAsync(Guid customerId, CancellationToken ct = default)
    {
        var cart = await LoadCartAsync(customerId, ct);
        return BuildSummary(cart);
    }

    public async Task<CartSummaryDto> AddItemAsync(Guid customerId, AddCartItemRequest request, CancellationToken ct = default)
    {
        if (request.Quantity <= 0)
            throw new AppException("Quantity must be at least 1.");

        // Marketplace read: cross the tenant boundary to fetch the variant.
        var variant = await _db.ProductVariants
            .IgnoreQueryFilters()
            .Include(v => v.Product)
            .FirstOrDefaultAsync(v => v.Id == request.ProductVariantId
                                      && v.StoreId == request.StoreId
                                      && v.IsActive, ct)
            ?? throw AppException.NotFound("Product variant not found.");

        var store = await _db.Stores.FirstOrDefaultAsync(s => s.Id == request.StoreId, ct)
                    ?? throw AppException.NotFound("Store not found.");
        if (store.Status != StoreStatus.Active)
            throw new AppException("This store is not currently accepting orders.");

        var cart = await LoadCartAsync(customerId, ct, createIfMissing: true);

        var existing = cart.Items.FirstOrDefault(i => i.ProductVariantId == request.ProductVariantId);
        var desiredQty = (existing?.Quantity ?? 0) + request.Quantity;
        if (desiredQty > variant.StockQuantity)
            throw new AppException($"Only {variant.StockQuantity} in stock.");

        if (existing is not null)
        {
            existing.Quantity = desiredQty;
            existing.UnitMrp = variant.Mrp;
            existing.UnitSellingPrice = variant.SellingPrice;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                CartId = cart.Id,
                StoreId = store.Id,
                StoreName = store.Name,
                ProductVariantId = variant.Id,
                ProductName = variant.Product!.Name,
                VariantName = variant.Name,
                Quantity = request.Quantity,
                UnitMrp = variant.Mrp,
                UnitSellingPrice = variant.SellingPrice
            });
        }

        await _db.SaveChangesAsync(ct);
        return BuildSummary(cart);
    }

    public async Task<CartSummaryDto> UpdateItemAsync(Guid customerId, Guid itemId, UpdateCartItemRequest request, CancellationToken ct = default)
    {
        var cart = await LoadCartAsync(customerId, ct);
        var item = cart.Items.FirstOrDefault(i => i.Id == itemId)
                   ?? throw AppException.NotFound("Cart item not found.");

        var removed = false;
        if (request.Quantity <= 0)
        {
            _db.CartItems.Remove(item);
            removed = true;
        }
        else
        {
            var variant = await _db.ProductVariants.IgnoreQueryFilters()
                .FirstOrDefaultAsync(v => v.Id == item.ProductVariantId, ct);
            if (variant is not null && request.Quantity > variant.StockQuantity)
                throw new AppException($"Only {variant.StockQuantity} in stock.");
            item.Quantity = request.Quantity;
        }

        await _db.SaveChangesAsync(ct);
        if (removed) cart.Items.Remove(item);
        return BuildSummary(cart);
    }

    public async Task<CartSummaryDto> RemoveItemAsync(Guid customerId, Guid itemId, CancellationToken ct = default)
    {
        var cart = await LoadCartAsync(customerId, ct);
        var item = cart.Items.FirstOrDefault(i => i.Id == itemId)
                   ?? throw AppException.NotFound("Cart item not found.");
        _db.CartItems.Remove(item);
        await _db.SaveChangesAsync(ct);

        cart.Items.Remove(item);
        return BuildSummary(cart);
    }

    public async Task<CartSummaryDto> ClearAsync(Guid customerId, CancellationToken ct = default)
    {
        var cart = await LoadCartAsync(customerId, ct);
        if (cart.Items.Count > 0)
        {
            _db.CartItems.RemoveRange(cart.Items);
            await _db.SaveChangesAsync(ct);
            cart.Items.Clear();
        }
        return BuildSummary(cart);
    }

    private async Task<Cart> LoadCartAsync(Guid customerId, CancellationToken ct, bool createIfMissing = false)
    {
        var cart = await _db.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId, ct);

        if (cart is null)
        {
            cart = new Cart { CustomerId = customerId };
            if (createIfMissing)
                _db.Carts.Add(cart);
        }
        return cart;
    }

    private CartSummaryDto BuildSummary(Cart cart)
    {
        var groups = cart.Items
            .GroupBy(i => i.StoreId)
            .Select(g =>
            {
                var items = g.Select(i => new CartItemDto(
                    i.Id, i.StoreId, i.StoreName, i.ProductVariantId, i.ProductName, i.VariantName,
                    i.Quantity, i.UnitMrp, i.UnitSellingPrice,
                    i.UnitSellingPrice * i.Quantity,
                    (i.UnitMrp - i.UnitSellingPrice) * i.Quantity)).ToList();

                var subtotal = items.Sum(x => x.LineTotal);
                var mrpTotal = g.Sum(i => i.UnitMrp * i.Quantity);
                return new CartStoreGroupDto(
                    g.Key, g.First().StoreName, items, subtotal, mrpTotal, mrpTotal - subtotal);
            })
            .OrderBy(g => g.StoreName)
            .ToList();

        var itemsSubtotal = groups.Sum(g => g.Subtotal);
        var totalMrp = groups.Sum(g => g.MrpTotal);
        var itemCount = cart.Items.Sum(i => i.Quantity);
        var storeCount = groups.Count;

        var deliveryFee = EstimateDelivery(itemsSubtotal, storeCount);

        return new CartSummaryDto(
            cart.Id, groups, storeCount, itemCount,
            itemsSubtotal, totalMrp, totalMrp - itemsSubtotal,
            deliveryFee, itemsSubtotal + deliveryFee);
    }

    private decimal EstimateDelivery(decimal itemsSubtotal, int storeCount)
    {
        if (storeCount == 0) return 0m;
        if (itemsSubtotal >= _delivery.FreeAboveOrderValue) return 0m;
        return _delivery.BaseFee + _delivery.PerAdditionalStoreFee * (storeCount - 1);
    }
}
