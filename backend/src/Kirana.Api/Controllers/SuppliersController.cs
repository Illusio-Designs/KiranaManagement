using Kirana.Application.Purchasing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kirana.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Owner,Manager,StockClerk")]
public class SuppliersController : ControllerBase
{
    private readonly IPurchaseService _purchase;

    public SuppliersController(IPurchaseService purchase)
    {
        _purchase = purchase;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SupplierDto>>> List(CancellationToken ct)
        => Ok(await _purchase.GetSuppliersAsync(ct));

    [HttpPost]
    public async Task<ActionResult<SupplierDto>> Create(CreateSupplierRequest request, CancellationToken ct)
        => Ok(await _purchase.CreateSupplierAsync(request, ct));
}
