using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kirana.WebApi.Models
{
    // A POS sale. Prices are tax-inclusive (Indian MRP retail); TaxTotal is the
    // GST component extracted from the total.
    public class SalesInvoice
    {
        public SalesInvoice()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Lines = new List<SalesLine>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }

        [MaxLength(40)]
        public string InvoiceNumber { get; set; }

        [MaxLength(200)]
        public string CustomerName { get; set; }

        public PaymentMode PaymentMode { get; set; }

        public decimal TaxableTotal { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal DiscountTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal ChangeDue { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<SalesLine> Lines { get; set; }
    }

    public class SalesLine
    {
        public SalesLine()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public Guid SalesInvoiceId { get; set; }
        public Guid ProductVariantId { get; set; }

        [MaxLength(200)]
        public string ProductName { get; set; }
        [MaxLength(120)]
        public string VariantName { get; set; }

        public int Quantity { get; set; }
        public decimal UnitMrp { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxRatePercent { get; set; }
        public decimal LineDiscount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal LineTotal { get; set; }
    }
}
