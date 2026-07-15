namespace Kirana.Application.Common.Interfaces;

public record GoogleUserInfo(string Email, string? Name, bool EmailVerified);

/// <summary>Validates a Google ID token (from Google Sign-In) and returns the verified profile.</summary>
public interface IGoogleTokenValidator
{
    /// <summary>Returns the verified user info, or null if the token is invalid.</summary>
    Task<GoogleUserInfo?> ValidateAsync(string idToken, CancellationToken ct = default);
}
