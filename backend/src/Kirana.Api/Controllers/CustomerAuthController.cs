using Kirana.Application.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kirana.Api.Controllers;

/// <summary>Marketplace customer authentication via phone OTP (PRD §5.6).</summary>
[ApiController]
[Route("api/customer-auth")]
[AllowAnonymous]
public class CustomerAuthController : ControllerBase
{
    private readonly ICustomerAuthService _auth;

    public CustomerAuthController(ICustomerAuthService auth)
    {
        _auth = auth;
    }

    /// <summary>Send a one-time code to the customer's phone.</summary>
    [HttpPost("request-otp")]
    public async Task<ActionResult<RequestOtpResponse>> RequestOtp(RequestOtpRequest request, CancellationToken ct)
        => Ok(await _auth.RequestOtpAsync(request, ct));

    /// <summary>Verify the OTP and return a customer JWT (creates the customer on first login).</summary>
    [HttpPost("verify-otp")]
    public async Task<ActionResult<CustomerAuthResponse>> VerifyOtp(VerifyOtpRequest request, CancellationToken ct)
        => Ok(await _auth.VerifyOtpAsync(request, ct));
}
