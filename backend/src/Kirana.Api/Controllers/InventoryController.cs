using Kirana.Application.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kirana.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Owner,Manager,StockClerk")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventory;

    public InventoryController(IInventoryService inventory)
    {
        _inventory = inventory;
    }

    [HttpPost("variants/{variantId:guid}/adjust")]
    public async Task<ActionResult<StockLedgerEntryDto>> Adjust(Guid variantId, AdjustStockRequest request, CancellationToken ct)
        => Ok(await _inventory.AdjustStockAsync(variantId, request, ct));

    [HttpGet("low-stock")]
    public async Task<ActionResult<IReadOnlyList<LowStockItemDto>>> LowStock(CancellationToken ct)
        => Ok(await _inventory.GetLowStockAsync(ct));

    [HttpGet("variants/{variantId:guid}/ledger")]
    public async Task<ActionResult<IReadOnlyList<StockLedgerEntryDto>>> Ledger(Guid variantId, CancellationToken ct)
        => Ok(await _inventory.GetLedgerAsync(variantId, ct));
}
