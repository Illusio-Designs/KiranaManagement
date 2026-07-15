using Kirana.Application.Auth.Dtos;

namespace Kirana.Application.Auth;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
}
