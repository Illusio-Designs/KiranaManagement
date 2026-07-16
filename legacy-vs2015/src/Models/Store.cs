using System;
using System.ComponentModel.DataAnnotations;

namespace Kirana.WebApi.Models
{
    // A grocery store (tenant). C# 6 / EF6 Code First.
    public class Store
    {
        public Store()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Status = StoreStatus.Pending;
        }

        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required, MaxLength(200)]
        public string OwnerName { get; set; }

        [Required, MaxLength(256)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(500)]
        public string AddressLine { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(20)]
        public string Gstin { get; set; }

        [MaxLength(20)]
        public string Pan { get; set; }

        public StoreStatus Status { get; set; }

        // Store geo-location (used to estimate delivery distance/ETA to the
        // customer). Nullable: seeded/known stores have it, others may not.
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }

        [MaxLength(500)]
        public string RejectionReason { get; set; }
    }
}
