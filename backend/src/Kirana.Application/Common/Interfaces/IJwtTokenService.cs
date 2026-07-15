using Kirana.Domain.Identity;

namespace Kirana.Application.Common.Interfaces;

public interface IJwtTokenService
{
    /// <summary>Issues a signed JWT carrying user id, email, role and (if any) store_id.</summary>
    (string Token, DateTime ExpiresAt) CreateToken(User user);
}
