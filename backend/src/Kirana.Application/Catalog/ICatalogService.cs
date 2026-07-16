namespace Kirana.Application.Catalog;

/// <summary>Store-owner catalog management: categories, products, and variants.</summary>
public interface ICatalogService
{
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default);

    Task<ProductDto> CreateProductAsync(CreateProductRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<ProductDto>> GetProductsAsync(CancellationToken ct = default);
    Task<ProductDto> GetProductAsync(Guid productId, CancellationToken ct = default);

    Task<ProductVariantDto> AddVariantAsync(Guid productId, CreateVariantRequest request, CancellationToken ct = default);
    Task<ProductVariantDto> UpdateVariantAsync(Guid variantId, UpdateVariantRequest request, CancellationToken ct = default);
}
