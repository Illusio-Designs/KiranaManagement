using Kirana.Application.Common.Exceptions;
using Kirana.Application.Common.Interfaces;
using Kirana.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Kirana.Application.Catalog;

public class CatalogService : ICatalogService
{
    private readonly IAppDbContext _db;
    private readonly ICurrentTenant _tenant;

    public CatalogService(IAppDbContext db, ICurrentTenant tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    private Guid StoreId => _tenant.HasTenant
        ? _tenant.StoreId
        : throw AppException.Unauthorized("No store context.");

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken ct = default)
    {
        var category = new Category
        {
            StoreId = StoreId,
            Name = request.Name.Trim(),
            ParentCategoryId = request.ParentCategoryId
        };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync(ct);
        return new CategoryDto(category.Id, category.Name, category.ParentCategoryId, category.IsActive);
    }

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default)
    {
        return await _db.Categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name, c.ParentCategoryId, c.IsActive))
            .ToListAsync(ct);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        if (request.Variants is null || request.Variants.Count == 0)
            throw new AppException("A product needs at least one variant.");

        var storeId = StoreId;
        var product = new Product
        {
            StoreId = storeId,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            CategoryId = request.CategoryId,
            Brand = request.Brand?.Trim(),
            ImageUrl = request.ImageUrl?.Trim()
        };

        foreach (var v in request.Variants)
            product.Variants.Add(BuildVariant(storeId, v));

        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);

        return MapProduct(product);
    }

    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync(CancellationToken ct = default)
    {
        var products = await _db.Products
            .Include(p => p.Variants)
            .OrderBy(p => p.Name)
            .ToListAsync(ct);
        return products.Select(MapProduct).ToList();
    }

    public async Task<ProductDto> GetProductAsync(Guid productId, CancellationToken ct = default)
    {
        var product = await _db.Products
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == productId, ct)
            ?? throw AppException.NotFound("Product not found.");
        return MapProduct(product);
    }

    public async Task<ProductVariantDto> AddVariantAsync(
        Guid productId, CreateVariantRequest request, CancellationToken ct = default)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId, ct)
                      ?? throw AppException.NotFound("Product not found.");

        var variant = BuildVariant(product.StoreId, request);
        variant.ProductId = product.Id;
        _db.ProductVariants.Add(variant);
        await _db.SaveChangesAsync(ct);

        return MapVariant(variant);
    }

    public async Task<ProductVariantDto> UpdateVariantAsync(
        Guid variantId, UpdateVariantRequest request, CancellationToken ct = default)
    {
        var variant = await _db.ProductVariants.FirstOrDefaultAsync(v => v.Id == variantId, ct)
                      ?? throw AppException.NotFound("Variant not found.");

        ValidatePricing(request.Mrp, request.SellingPrice);

        variant.Name = request.Name.Trim();
        variant.Mrp = request.Mrp;
        variant.SellingPrice = request.SellingPrice;
        variant.TaxRatePercent = request.TaxRatePercent;
        variant.ReorderLevel = request.ReorderLevel;
        variant.IsActive = request.IsActive;
        await _db.SaveChangesAsync(ct);

        return MapVariant(variant);
    }

    private static ProductVariant BuildVariant(Guid storeId, CreateVariantRequest v)
    {
        ValidatePricing(v.Mrp, v.SellingPrice);
        return new ProductVariant
        {
            StoreId = storeId,
            Name = v.Name.Trim(),
            Sku = v.Sku?.Trim(),
            Barcode = v.Barcode?.Trim(),
            Unit = v.Unit,
            PackSize = v.PackSize <= 0 ? 1m : v.PackSize,
            Mrp = v.Mrp,
            SellingPrice = v.SellingPrice,
            TaxRatePercent = v.TaxRatePercent,
            StockQuantity = v.StockQuantity < 0 ? 0 : v.StockQuantity,
            ReorderLevel = v.ReorderLevel < 0 ? 0 : v.ReorderLevel
        };
    }

    private static void ValidatePricing(decimal mrp, decimal sellingPrice)
    {
        if (mrp <= 0)
            throw new AppException("MRP must be greater than zero.");
        if (sellingPrice <= 0)
            throw new AppException("Selling price must be greater than zero.");
        if (sellingPrice > mrp)
            throw new AppException("Selling price cannot exceed MRP.");
    }

    private static ProductDto MapProduct(Product p) => new(
        p.Id, p.Name, p.Description, p.CategoryId, p.Brand, p.ImageUrl, p.IsActive,
        p.Variants.Select(MapVariant).ToList());

    private static ProductVariantDto MapVariant(ProductVariant v) => new(
        v.Id, v.ProductId, v.Name, v.Sku, v.Barcode, v.Unit, v.PackSize,
        v.Mrp, v.SellingPrice, v.DiscountAmount, v.DiscountPercent, v.TaxRatePercent,
        v.StockQuantity, v.ReorderLevel, v.IsActive);
}
