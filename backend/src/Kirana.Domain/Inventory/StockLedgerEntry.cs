using Kirana.Domain.Common;
using Kirana.Domain.Common.Enums;

namespace Kirana.Domain.Inventory;

/// <summary>An immutable record of a stock movement for a variant (audit trail).</summary>
public class StockLedgerEntry : BaseEntity, ITenantEntity
{
    public Guid StoreId { get; set; }
    public Guid ProductVariantId { get; set; }

    public int ChangeQuantity { get; set; }   // + in, - out
    public int BalanceAfter { get; set; }
    public StockMovementType MovementType { get; set; }
    public string? Reason { get; set; }
}
