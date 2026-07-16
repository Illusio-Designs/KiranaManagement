using Kirana.Application.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kirana.Api.Controllers;

/// <summary>In-store POS billing (PRD §5.5).</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Owner,Manager,Cashier")]
public class SalesController : ControllerBase
{
    private readonly ISalesService _sales;

    public SalesController(ISalesService sales)
    {
        _sales = sales;
    }

    [HttpPost]
    public async Task<ActionResult<SalesInvoiceDto>> Create(CreateSaleRequest request, CancellationToken ct)
    {
        var invoice = await _sales.CreateSaleAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = invoice.Id }, invoice);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SalesInvoiceDto>>> List(CancellationToken ct)
        => Ok(await _sales.GetSalesAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SalesInvoiceDto>> Get(Guid id, CancellationToken ct)
        => Ok(await _sales.GetSaleAsync(id, ct));
}
