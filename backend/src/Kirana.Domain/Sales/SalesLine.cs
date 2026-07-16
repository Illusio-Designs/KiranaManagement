using Kirana.Domain.Common;

namespace Kirana.Domain.Sales;

public class SalesLine : BaseEntity, ITenantEntity
{
    public Guid StoreId { get; set; }
    public Guid SalesInvoiceId { get; set; }
    public SalesInvoice? SalesInvoice { get; set; }

    public Guid ProductVariantId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string VariantName { get; set; } = string.Empty;

    public int Quantity { get; set; }
    public decimal UnitMrp { get; set; }
    public decimal UnitPrice { get; set; }        // actual price charged (≤ MRP)
    public decimal TaxRatePercent { get; set; }

    public decimal LineDiscount { get; set; }      // (MRP - price) * qty
    public decimal TaxAmount { get; set; }         // GST component of the line
    public decimal LineTotal { get; set; }         // price * qty (tax-inclusive)
}
