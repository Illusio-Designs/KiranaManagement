using Kirana.Application.Stores;
using Kirana.Application.Stores.Dtos;
using Kirana.Domain.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kirana.Api.Controllers;

/// <summary>Platform Super Admin console — store approval workflow (PRD §5.12).</summary>
[ApiController]
[Route("api/admin")]
[Authorize(Roles = nameof(UserRole.SuperAdmin))]
public class AdminController : ControllerBase
{
    private readonly IStoreService _stores;

    public AdminController(IStoreService stores)
    {
        _stores = stores;
    }

    [HttpGet("stores")]
    public async Task<ActionResult<IReadOnlyList<StoreDto>>> GetAll(CancellationToken ct)
        => Ok(await _stores.GetAllAsync(ct));

    [HttpGet("stores/pending")]
    public async Task<ActionResult<IReadOnlyList<StoreDto>>> GetPending(CancellationToken ct)
        => Ok(await _stores.GetPendingAsync(ct));

    [HttpPost("stores/{id:guid}/approve")]
    public async Task<ActionResult<StoreDto>> Approve(Guid id, CancellationToken ct)
        => Ok(await _stores.ApproveAsync(id, ct));

    [HttpPost("stores/{id:guid}/reject")]
    public async Task<ActionResult<StoreDto>> Reject(Guid id, RejectStoreRequest request, CancellationToken ct)
        => Ok(await _stores.RejectAsync(id, request.Reason, ct));
}
