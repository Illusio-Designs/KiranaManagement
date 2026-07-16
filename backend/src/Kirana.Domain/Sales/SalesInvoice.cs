using Kirana.Domain.Common;
using Kirana.Domain.Common.Enums;

namespace Kirana.Domain.Sales;

/// <summary>
/// An in-store (POS) sale. Prices are treated as tax-inclusive (Indian MRP retail):
/// <see cref="GrandTotal"/> is what the customer pays; <see cref="TaxTotal"/> is the
/// GST component extracted from it. Tenant-scoped.
/// </summary>
public class SalesInvoice : BaseEntity, ITenantEntity
{
    public Guid StoreId { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }

    public decimal TaxableTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal DiscountTotal { get; set; }   // vs MRP
    public decimal GrandTotal { get; set; }

    public PaymentMode PaymentMode { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal ChangeDue { get; set; }

    public SaleStatus Status { get; set; } = SaleStatus.Completed;

    public ICollection<SalesLine> Lines { get; set; } = new List<SalesLine>();
}
