using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kirana.WebApi.Models
{
    // A catalog product. Prices/stock live on its variants.
    public class Product
    {
        public Product()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            Status = ProductStatus.Pending;   // goes live only after admin approval
            Variants = new List<ProductVariant>();
        }

        [Key]
        public Guid Id { get; set; }

        public Guid StoreId { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(150)]
        public string Brand { get; set; }

        // Consumer-facing category used for search/filter on the marketplace
        // (e.g. "Vegetables", "Fruits", "Dairy & Eggs", "Snacks", "Grains").
        [MaxLength(100)]
        public string Category { get; set; }

        // Product image. May be set directly, or inherited from the shared
        // master catalog image (keyed by product name) when left blank.
        [MaxLength(1000)]
        public string ImageUrl { get; set; }

        public bool IsActive { get; set; }

        // Marketplace moderation: Pending until a SuperAdmin approves it.
        public ProductStatus Status { get; set; }
        [MaxLength(500)]
        public string RejectionReason { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<ProductVariant> Variants { get; set; }
    }
}
