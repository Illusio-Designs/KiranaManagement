using Kirana.Application.Auth.Dtos;

namespace Kirana.Application.Auth;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);

    /// <summary>Store-owner login via Google Sign-In (verifies the Google ID token).</summary>
    Task<AuthResponse> LoginWithGoogleAsync(GoogleLoginRequest request, CancellationToken ct = default);
}
