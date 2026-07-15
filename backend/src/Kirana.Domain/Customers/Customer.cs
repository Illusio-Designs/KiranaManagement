using Kirana.Domain.Common;

namespace Kirana.Domain.Customers;

/// <summary>
/// A marketplace shopper. Platform-level (shops across all stores). Authenticates
/// via phone OTP — no password. See PRD §5.6 and the OTP auth flow.
/// </summary>
public class Customer : BaseEntity
{
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
