using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kirana.WebApi.Models
{
    // A sellable variant (e.g. "1 kg", "500 g"). Has its own MRP + selling price;
    // the discount is derived from the two.
    public class ProductVariant
    {
        public ProductVariant()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            PackSize = 1m;
        }

        [Key]
        public Guid Id { get; set; }

        public Guid StoreId { get; set; }
        public Guid ProductId { get; set; }

        // Navigation back to the parent product (needed for .Include(v => v.Product)).
        public virtual Product Product { get; set; }

        [MaxLength(120)]
        public string Name { get; set; }

        [MaxLength(60)]
        public string Sku { get; set; }

        [MaxLength(60)]
        public string Barcode { get; set; }

        public UnitOfMeasure Unit { get; set; }

        [Column(TypeName = "decimal")]
        public decimal PackSize { get; set; }

        // Maximum Retail Price (printed price)
        public decimal Mrp { get; set; }

        // Actual selling price (<= MRP)
        public decimal SellingPrice { get; set; }

        public decimal TaxRatePercent { get; set; }

        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Derived, not stored in the database.
        [NotMapped]
        public decimal DiscountAmount
        {
            get { return Mrp - SellingPrice > 0 ? Mrp - SellingPrice : 0m; }
        }

        [NotMapped]
        public decimal DiscountPercent
        {
            get { return Mrp > 0 ? Math.Round(DiscountAmount / Mrp * 100m, 2) : 0m; }
        }
    }
}
