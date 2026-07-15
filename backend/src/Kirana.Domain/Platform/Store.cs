using Kirana.Domain.Common;
using Kirana.Domain.Common.Enums;

namespace Kirana.Domain.Platform;

/// <summary>
/// A grocery store = a tenant. Platform-level entity (NOT tenant-filtered): it
/// is the root that other rows are scoped to. See PRD §5.1, §5.2.
/// </summary>
public class Store : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? Pincode { get; set; }
    public string? Gstin { get; set; }

    public StoreStatus Status { get; set; } = StoreStatus.Pending;

    // Approval workflow (PRD FR-1.4)
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }
}
