using Kirana.Application.Auth;
using Kirana.Application.Auth.Dtos;
using Kirana.Application.Stores;
using Kirana.Application.Stores.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kirana.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IStoreService _stores;
    private readonly IAuthService _auth;

    public AuthController(IStoreService stores, IAuthService auth)
    {
        _stores = stores;
        _auth = auth;
    }

    /// <summary>Public store self-registration → status Pending (PRD FR-1.1).</summary>
    [AllowAnonymous]
    [HttpPost("register-store")]
    public async Task<ActionResult<StoreDto>> RegisterStore(RegisterStoreRequest request, CancellationToken ct)
    {
        var store = await _stores.RegisterAsync(request, ct);
        return Created($"/api/stores/{store.Id}", store);
    }

    /// <summary>Login → JWT carrying user id, role and store_id.</summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var response = await _auth.LoginAsync(request, ct);
        return Ok(response);
    }

    /// <summary>Store-owner sign-in with Google (send the Google ID token).</summary>
    [AllowAnonymous]
    [HttpPost("google")]
    public async Task<ActionResult<AuthResponse>> Google(GoogleLoginRequest request, CancellationToken ct)
    {
        var response = await _auth.LoginWithGoogleAsync(request, ct);
        return Ok(response);
    }
}
