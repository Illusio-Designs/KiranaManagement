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

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<ProductVariant> Variants { get; set; }
    }
}
