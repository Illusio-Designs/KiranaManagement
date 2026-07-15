using System.Security.Claims;
using Kirana.Application.Common.Interfaces;
using Kirana.Domain.Common.Enums;
using Kirana.Infrastructure.Identity;

namespace Kirana.Api.Services;

/// <summary>
/// Resolves the current user and tenant from the request's JWT claims.
/// Registered as both <see cref="ICurrentUser"/> and <see cref="ICurrentTenant"/>.
/// </summary>
public class CurrentUser : ICurrentUser, ICurrentTenant
{
    private readonly ClaimsPrincipal? _principal;

    public CurrentUser(IHttpContextAccessor accessor)
    {
        _principal = accessor.HttpContext?.User;
    }

    public bool IsAuthenticated => _principal?.Identity?.IsAuthenticated == true;

    public Guid? UserId
    {
        get
        {
            var value = _principal?.FindFirstValue("sub")
                        ?? _principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public UserRole? Role
    {
        get
        {
            var value = _principal?.FindFirstValue(ClaimTypes.Role);
            return Enum.TryParse<UserRole>(value, out var role) ? role : null;
        }
    }

    // ICurrentTenant
    public Guid StoreId
    {
        get
        {
            var value = _principal?.FindFirstValue(JwtTokenService.StoreIdClaim);
            return Guid.TryParse(value, out var id) ? id : Guid.Empty;
        }
    }

    public bool HasTenant => StoreId != Guid.Empty;
}
