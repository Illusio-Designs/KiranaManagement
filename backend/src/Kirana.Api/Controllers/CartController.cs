using Kirana.Application.Carts;
using Kirana.Application.Common.Exceptions;
using Kirana.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kirana.Api.Controllers;

/// <summary>Marketplace cart for a logged-in customer (multi-store, one basket).</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class CartController : ControllerBase
{
    private readonly ICartService _cart;
    private readonly ICurrentUser _currentUser;

    public CartController(ICartService cart, ICurrentUser currentUser)
    {
        _cart = cart;
        _currentUser = currentUser;
    }

    private Guid CustomerId => _currentUser.UserId
        ?? throw AppException.Unauthorized("Not authenticated.");

    [HttpGet]
    public async Task<ActionResult<CartSummaryDto>> Get(CancellationToken ct)
        => Ok(await _cart.GetSummaryAsync(CustomerId, ct));

    [HttpPost("items")]
    public async Task<ActionResult<CartSummaryDto>> AddItem(AddCartItemRequest request, CancellationToken ct)
        => Ok(await _cart.AddItemAsync(CustomerId, request, ct));

    [HttpPut("items/{itemId:guid}")]
    public async Task<ActionResult<CartSummaryDto>> UpdateItem(Guid itemId, UpdateCartItemRequest request, CancellationToken ct)
        => Ok(await _cart.UpdateItemAsync(CustomerId, itemId, request, ct));

    [HttpDelete("items/{itemId:guid}")]
    public async Task<ActionResult<CartSummaryDto>> RemoveItem(Guid itemId, CancellationToken ct)
        => Ok(await _cart.RemoveItemAsync(CustomerId, itemId, ct));

    [HttpDelete]
    public async Task<ActionResult<CartSummaryDto>> Clear(CancellationToken ct)
        => Ok(await _cart.ClearAsync(CustomerId, ct));
}
