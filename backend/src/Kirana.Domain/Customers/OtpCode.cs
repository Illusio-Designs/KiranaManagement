using Kirana.Domain.Common;

namespace Kirana.Domain.Customers;

/// <summary>A one-time login code issued to a phone number. The code is stored hashed.</summary>
public class OtpCode : BaseEntity
{
    public string Phone { get; set; } = string.Empty;
    public string CodeHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsConsumed { get; set; }
    public int Attempts { get; set; }
}
