using Kirana.Domain.Common.Enums;

namespace Kirana.Application.Common.Interfaces;

/// <summary>The authenticated user for the current request (from the JWT).</summary>
public interface ICurrentUser
{
    Guid? UserId { get; }
    UserRole? Role { get; }
    bool IsAuthenticated { get; }
}
