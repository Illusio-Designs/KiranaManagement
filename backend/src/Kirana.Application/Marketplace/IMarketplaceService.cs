using Kirana.Application.Catalog;

namespace Kirana.Application.Marketplace;

/// <summary>
/// Customer-facing, cross-store reads. Bypasses the tenant filter (marketplace
/// spans all stores) but only exposes active products from active stores.
/// </summary>
public interface IMarketplaceService
{
    Task<IReadOnlyList<ProductDto>> GetStoreProductsAsync(Guid storeId, CancellationToken ct = default);
}
