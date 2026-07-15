using Kirana.Domain.Common.Enums;

namespace Kirana.Application.Auth.Dtos;

public record LoginRequest(string Email, string Password);

/// <summary>Store-owner sign-in with Google — send the ID token from Google Sign-In.</summary>
public record GoogleLoginRequest(string IdToken);

public record AuthResponse(
    string Token,
    DateTime ExpiresAt,
    Guid UserId,
    string Email,
    string FullName,
    UserRole Role,
    Guid? StoreId);
