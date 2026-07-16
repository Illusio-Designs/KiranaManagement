using Kirana.Application.Catalog;
using Kirana.Application.Common.Exceptions;
using Kirana.Application.Common.Interfaces;
using Kirana.Domain.Catalog;
using Kirana.Domain.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kirana.Application.Marketplace;

public class MarketplaceService : IMarketplaceService
{
    private readonly IAppDbContext _db;

    public MarketplaceService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ProductDto>> GetStoreProductsAsync(Guid storeId, CancellationToken ct = default)
    {
        var storeActive = await _db.Stores.AnyAsync(s => s.Id == storeId && s.Status == StoreStatus.Active, ct);
        if (!storeActive)
            throw AppException.NotFound("Store not found or not active.");

        // IgnoreQueryFilters: marketplace reads cross the tenant boundary.
        var products = await _db.Products
            .IgnoreQueryFilters()
            .Where(p => p.StoreId == storeId && p.IsActive)
            .Include(p => p.Variants.Where(v => v.IsActive))
            .OrderBy(p => p.Name)
            .ToListAsync(ct);

        return products.Select(MapProduct).ToList();
    }

    private static ProductDto MapProduct(Product p) => new(
        p.Id, p.Name, p.Description, p.CategoryId, p.Brand, p.ImageUrl, p.IsActive,
        p.Variants.Select(MapVariant).ToList());

    private static ProductVariantDto MapVariant(ProductVariant v) => new(
        v.Id, v.ProductId, v.Name, v.Sku, v.Barcode, v.Unit, v.PackSize,
        v.Mrp, v.SellingPrice, v.DiscountAmount, v.DiscountPercent, v.TaxRatePercent,
        v.StockQuantity, v.ReorderLevel, v.IsActive);
}
