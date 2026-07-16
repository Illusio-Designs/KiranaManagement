using Kirana.Application.Delivery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kirana.Api.Controllers;

/// <summary>
/// Receives 3PL partner status callbacks. In production the request signature
/// must be verified and events treated idempotently (see DELIVERY_INTEGRATIONS.md).
/// </summary>
[ApiController]
[AllowAnonymous]
public class DeliveryWebhooksController : ControllerBase
{
    private readonly IDeliveryService _delivery;

    public DeliveryWebhooksController(IDeliveryService delivery)
    {
        _delivery = delivery;
    }

    [HttpPost("/api/webhooks/delivery/{provider}")]
    public async Task<IActionResult> Receive(string provider, DeliveryWebhookRequest payload, CancellationToken ct)
    {
        await _delivery.HandleWebhookAsync(provider, payload, ct);
        return Ok(new { received = true });
    }
}
