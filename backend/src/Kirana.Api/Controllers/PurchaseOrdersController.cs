using Kirana.Application.Purchasing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kirana.Api.Controllers;

/// <summary>Purchase orders + goods receipt (PRD §5.4).</summary>
[ApiController]
[Route("api/purchase-orders")]
[Authorize(Roles = "Owner,Manager,StockClerk")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IPurchaseService _purchase;

    public PurchaseOrdersController(IPurchaseService purchase)
    {
        _purchase = purchase;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PurchaseOrderDto>>> List(CancellationToken ct)
        => Ok(await _purchase.GetPurchaseOrdersAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PurchaseOrderDto>> Get(Guid id, CancellationToken ct)
        => Ok(await _purchase.GetPurchaseOrderAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<PurchaseOrderDto>> Create(CreatePurchaseOrderRequest request, CancellationToken ct)
    {
        var po = await _purchase.CreatePurchaseOrderAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = po.Id }, po);
    }

    /// <summary>Goods receipt — marks the PO received and increases stock.</summary>
    [HttpPost("{id:guid}/receive")]
    public async Task<ActionResult<PurchaseOrderDto>> Receive(Guid id, CancellationToken ct)
        => Ok(await _purchase.ReceivePurchaseOrderAsync(id, ct));
}
