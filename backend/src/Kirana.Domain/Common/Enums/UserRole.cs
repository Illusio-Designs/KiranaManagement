namespace Kirana.Domain.Common.Enums;

/// <summary>Coarse roles for Phase 0 — see PRD §3.2 (fine-grained RBAC comes later).</summary>
public enum UserRole
{
    SuperAdmin = 0,   // platform operator (no store)
    Owner = 1,        // owns a store
    Manager = 2,
    Cashier = 3,
    StockClerk = 4,
    Accountant = 5
}
