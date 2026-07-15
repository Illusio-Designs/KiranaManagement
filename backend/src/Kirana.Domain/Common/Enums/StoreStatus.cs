namespace Kirana.Domain.Common.Enums;

/// <summary>Lifecycle of a store (tenant) on the platform — see PRD FR-1.3.</summary>
public enum StoreStatus
{
    Pending = 0,
    Active = 1,
    Suspended = 2,
    Rejected = 3,
    Closed = 4
}
