using Kirana.Domain.Common.Enums;

namespace Kirana.Application.Auth.Dtos;

public record LoginRequest(string Email, string Password);

public record AuthResponse(
    string Token,
    DateTime ExpiresAt,
    Guid UserId,
    string Email,
    string FullName,
    UserRole Role,
    Guid? StoreId);
