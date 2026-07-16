using System;
using System.ComponentModel.DataAnnotations;

namespace Kirana.WebApi.Models
{
    // Master product image shared across stores, keyed by the normalized product
    // name. The first store to add a product with an image defines the master;
    // any later product with the same name and no image of its own reuses it.
    public class CatalogImage
    {
        public CatalogImage()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        [Key]
        public Guid Id { get; set; }

        // Normalized lookup key (lower-cased, trimmed, single-spaced product name).
        [Required, MaxLength(200)]
        public string NameKey { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }          // original display name

        [MaxLength(160)]
        public string ImageName { get; set; }     // derived file/image name

        [Required, MaxLength(1000)]
        public string ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        // Turns "Fresh  Apples " -> "fresh apples" so the same product from any
        // store maps to one master record.
        public static string Key(string name)
        {
            var s = (name ?? "").Trim().ToLowerInvariant();
            while (s.Contains("  ")) s = s.Replace("  ", " ");
            return s;
        }

        // Derives a friendly image name from the product name, e.g.
        // "Fresh Apples" -> "fresh-apples".
        public static string DeriveImageName(string name)
        {
            var key = Key(name);
            return key.Replace(' ', '-');
        }
    }
}
