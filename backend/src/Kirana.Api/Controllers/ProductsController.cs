using Kirana.Application.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kirana.Api.Controllers;

/// <summary>
/// Store catalog: products and their variants (each variant has MRP + selling
/// price; the discount is derived). Tenant-scoped to the caller's store.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Owner,Manager")]
public class ProductsController : ControllerBase
{
    private readonly ICatalogService _catalog;

    public ProductsController(ICatalogService catalog)
    {
        _catalog = catalog;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> List(CancellationToken ct)
        => Ok(await _catalog.GetProductsAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Get(Guid id, CancellationToken ct)
        => Ok(await _catalog.GetProductAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductRequest request, CancellationToken ct)
    {
        var product = await _catalog.CreateProductAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }

    [HttpPost("{id:guid}/variants")]
    public async Task<ActionResult<ProductVariantDto>> AddVariant(Guid id, CreateVariantRequest request, CancellationToken ct)
        => Ok(await _catalog.AddVariantAsync(id, request, ct));

    [HttpPut("/api/variants/{variantId:guid}")]
    public async Task<ActionResult<ProductVariantDto>> UpdateVariant(Guid variantId, UpdateVariantRequest request, CancellationToken ct)
        => Ok(await _catalog.UpdateVariantAsync(variantId, request, ct));
}
