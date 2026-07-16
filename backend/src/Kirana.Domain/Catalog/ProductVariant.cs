using Kirana.Domain.Common;
using Kirana.Domain.Common.Enums;

namespace Kirana.Domain.Catalog;

/// <summary>
/// A specific sellable variant of a product (e.g. "1 kg", "500 g", "6-pack").
/// Carries its own MRP and selling price; the discount is derived from the two.
/// Stock is tracked here.
/// </summary>
public class ProductVariant : BaseEntity, ITenantEntity
{
    public Guid StoreId { get; set; }
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }

    public string Name { get; set; } = string.Empty;   // e.g. "1 kg"
    public string? Sku { get; set; }
    public string? Barcode { get; set; }

    public UnitOfMeasure Unit { get; set; } = UnitOfMeasure.Piece;
    public decimal PackSize { get; set; } = 1m;         // e.g. 1, 0.5

    /// <summary>Maximum Retail Price (printed price).</summary>
    public decimal Mrp { get; set; }

    /// <summary>Actual selling price (≤ MRP). Discount is the gap.</summary>
    public decimal SellingPrice { get; set; }

    public decimal TaxRatePercent { get; set; }

    public int StockQuantity { get; set; }
    public int ReorderLevel { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>Absolute discount vs MRP (never negative). Not persisted.</summary>
    public decimal DiscountAmount => Mrp - SellingPrice > 0 ? Mrp - SellingPrice : 0m;

    /// <summary>Discount as a percentage of MRP. Not persisted.</summary>
    public decimal DiscountPercent => Mrp > 0 ? Math.Round(DiscountAmount / Mrp * 100m, 2) : 0m;
}
