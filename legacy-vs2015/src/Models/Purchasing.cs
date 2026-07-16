using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kirana.WebApi.Models
{
    public class Supplier
    {
        public Supplier()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        [Key]
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(20)]
        public string Phone { get; set; }
        [MaxLength(256)]
        public string Email { get; set; }
        [MaxLength(20)]
        public string Gstin { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PurchaseOrder
    {
        public PurchaseOrder()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Status = PurchaseStatus.Draft;
            Lines = new List<PurchaseOrderLine>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public Guid SupplierId { get; set; }

        [MaxLength(40)]
        public string PoNumber { get; set; }
        public PurchaseStatus Status { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReceivedAt { get; set; }

        public virtual ICollection<PurchaseOrderLine> Lines { get; set; }
    }

    public class PurchaseOrderLine
    {
        public PurchaseOrderLine()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public Guid ProductVariantId { get; set; }

        [MaxLength(160)]
        public string VariantName { get; set; }

        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LineTotal { get; set; }
    }
}
