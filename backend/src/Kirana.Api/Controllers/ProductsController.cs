using Kirana.Application.Common.Interfaces;
using Kirana.Domain.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kirana.Api.Controllers;

/// <summary>
/// Minimal catalog endpoint included in Phase 0 to prove multi-tenant isolation:
/// products are auto-scoped to the caller's store by the global query filter, so
/// a store only ever sees its own products. Full catalog CRUD lands in Phase 1.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IAppDbContext _db;

    public ProductsController(IAppDbContext db)
    {
        _db = db;
    }

    public record CreateProductRequest(string Name, string? Sku, decimal Price, decimal TaxRate, int StockQuantity);
    public record ProductDto(Guid Id, string Name, string? Sku, decimal Price, decimal TaxRate, int StockQuantity);

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> List(CancellationToken ct)
    {
        // No StoreId filter needed here — the tenant query filter applies automatically.
        var products = await _db.Products
            .OrderBy(p => p.Name)
            .Select(p => new ProductDto(p.Id, p.Name, p.Sku, p.Price, p.TaxRate, p.StockQuantity))
            .ToListAsync(ct);
        return Ok(products);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductRequest request, CancellationToken ct)
    {
        // StoreId is stamped automatically on save from the current tenant.
        var product = new Product
        {
            Name = request.Name.Trim(),
            Sku = request.Sku?.Trim(),
            Price = request.Price,
            TaxRate = request.TaxRate,
            StockQuantity = request.StockQuantity
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);

        var dto = new ProductDto(product.Id, product.Name, product.Sku, product.Price, product.TaxRate, product.StockQuantity);
        return CreatedAtAction(nameof(List), new { id = product.Id }, dto);
    }
}
