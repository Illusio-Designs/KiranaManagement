using Kirana.Application.Common.Exceptions;
using Kirana.Application.Common.Interfaces;
using Kirana.Application.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kirana.Api.Controllers;

/// <summary>Customer checkout and order tracking (PRD §4.4, §5.7).</summary>
[ApiController]
[Authorize(Roles = "Customer")]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkout;
    private readonly ICurrentUser _currentUser;

    public CheckoutController(ICheckoutService checkout, ICurrentUser currentUser)
    {
        _checkout = checkout;
        _currentUser = currentUser;
    }

    private Guid CustomerId => _currentUser.UserId
        ?? throw AppException.Unauthorized("Not authenticated.");

    /// <summary>Convert the cart into a single order split across its stores.</summary>
    [HttpPost("/api/checkout")]
    public async Task<ActionResult<OrderDto>> Checkout(CheckoutRequest request, CancellationToken ct)
    {
        var order = await _checkout.CheckoutAsync(CustomerId, request, ct);
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    [HttpGet("/api/orders")]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> MyOrders(CancellationToken ct)
        => Ok(await _checkout.GetMyOrdersAsync(CustomerId, ct));

    [HttpGet("/api/orders/{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetOrder(Guid id, CancellationToken ct)
        => Ok(await _checkout.GetMyOrderAsync(CustomerId, id, ct));
}
