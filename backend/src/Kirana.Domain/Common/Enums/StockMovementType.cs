namespace Kirana.Domain.Common.Enums;

/// <summary>Reason a variant's stock changed (for the stock ledger).</summary>
public enum StockMovementType
{
    Adjustment = 0,
    PurchaseReceipt = 1,
    Sale = 2,
    ReturnIn = 3,
    ReturnOut = 4
}
