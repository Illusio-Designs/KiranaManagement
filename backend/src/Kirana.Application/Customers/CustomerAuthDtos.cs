namespace Kirana.Application.Customers;

public record RequestOtpRequest(string Phone);

public record RequestOtpResponse(string Phone, int ExpiresInSeconds, string Message);

public record VerifyOtpRequest(string Phone, string Code);

public record CustomerAuthResponse(
    string Token,
    DateTime ExpiresAt,
    Guid CustomerId,
    string Phone,
    string? FullName);
