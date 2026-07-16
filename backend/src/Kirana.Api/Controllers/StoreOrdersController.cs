using Kirana.Application.Orders;
using Kirana.Domain.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kirana.Api.Controllers;

/// <summary>A store's queue for its parts of marketplace orders (PRD §5.7).</summary>
[ApiController]
[Route("api/store-orders")]
[Authorize(Roles = "Owner,Manager,Cashier,StockClerk")]
public class StoreOrdersController : ControllerBase
{
    private readonly IStoreOrderService _orders;

    public StoreOrdersController(IStoreOrderService orders)
    {
        _orders = orders;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StoreOrderDto>>> List([FromQuery] StoreOrderStatus? status, CancellationToken ct)
        => Ok(await _orders.GetStoreOrdersAsync(status, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StoreOrderDto>> Get(Guid id, CancellationToken ct)
        => Ok(await _orders.GetStoreOrderAsync(id, ct));

    [HttpPost("{id:guid}/accept")]
    public async Task<ActionResult<StoreOrderDto>> Accept(Guid id, CancellationToken ct)
        => Ok(await _orders.AcceptAsync(id, ct));

    [HttpPost("{id:guid}/pack")]
    public async Task<ActionResult<StoreOrderDto>> Pack(Guid id, CancellationToken ct)
        => Ok(await _orders.PackAsync(id, ct));

    [HttpPost("{id:guid}/ready")]
    public async Task<ActionResult<StoreOrderDto>> Ready(Guid id, CancellationToken ct)
        => Ok(await _orders.MarkReadyAsync(id, ct));

    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<StoreOrderDto>> Cancel(Guid id, CancellationToken ct)
        => Ok(await _orders.CancelAsync(id, ct));
}
