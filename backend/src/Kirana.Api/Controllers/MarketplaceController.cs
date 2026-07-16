using Kirana.Application.Catalog;
using Kirana.Application.Marketplace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kirana.Api.Controllers;

/// <summary>Customer-facing marketplace browse (cross-store).</summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class MarketplaceController : ControllerBase
{
    private readonly IMarketplaceService _marketplace;

    public MarketplaceController(IMarketplaceService marketplace)
    {
        _marketplace = marketplace;
    }

    [HttpGet("stores/{storeId:guid}/products")]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> StoreProducts(Guid storeId, CancellationToken ct)
        => Ok(await _marketplace.GetStoreProductsAsync(storeId, ct));
}
