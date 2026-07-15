using Kirana.Application.Common.Exceptions;
using Kirana.Application.Common.Interfaces;
using Kirana.Application.Stores;
using Kirana.Application.Stores.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kirana.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StoresController : ControllerBase
{
    private readonly IStoreService _stores;
    private readonly ICurrentTenant _tenant;

    public StoresController(IStoreService stores, ICurrentTenant tenant)
    {
        _stores = stores;
        _tenant = tenant;
    }

    /// <summary>The authenticated user's own store.</summary>
    [HttpGet("me")]
    public async Task<ActionResult<StoreDto>> MyStore(CancellationToken ct)
    {
        if (!_tenant.HasTenant)
            throw AppException.NotFound("No store is associated with this account.");

        return Ok(await _stores.GetByIdAsync(_tenant.StoreId, ct));
    }
}
