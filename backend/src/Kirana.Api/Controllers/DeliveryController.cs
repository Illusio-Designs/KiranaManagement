using Kirana.Application.Delivery;
using Kirana.Domain.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kirana.Api.Controllers;

/// <summary>Delivery for a marketplace order via a 3PL partner (PRD §5.8).</summary>
[ApiController]
[Authorize]
public class DeliveryController : ControllerBase
{
    private readonly IDeliveryService _delivery;

    public DeliveryController(IDeliveryService delivery)
    {
        _delivery = delivery;
    }

    /// <summary>Platform dispatch: book a 3PL delivery for a ready order.</summary>
    [Authorize(Roles = nameof(UserRole.SuperAdmin))]
    [HttpPost("/api/orders/{orderId:guid}/delivery/book")]
    public async Task<ActionResult<DeliveryDto>> Book(Guid orderId, CancellationToken ct)
        => Ok(await _delivery.BookAsync(orderId, ct));

    /// <summary>Delivery status / tracking for an order.</summary>
    [HttpGet("/api/orders/{orderId:guid}/delivery")]
    public async Task<ActionResult<DeliveryDto>> Get(Guid orderId, CancellationToken ct)
        => Ok(await _delivery.GetForOrderAsync(orderId, ct));
}
