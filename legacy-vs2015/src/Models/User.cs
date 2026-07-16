using System;
using System.ComponentModel.DataAnnotations;

namespace Kirana.WebApi.Models
{
    public class User
    {
        public User()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(256)]
        public string Email { get; set; }

        [Required, MaxLength(500)]
        public string PasswordHash { get; set; }

        [MaxLength(200)]
        public string FullName { get; set; }

        public UserRole Role { get; set; }

        // Null for the platform SuperAdmin; set for store staff.
        public Guid? StoreId { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Simple session token issued on login and sent back in the
        // X-Auth-Token header (course-friendly auth, no OWIN needed).
        [MaxLength(64)]
        public string SessionToken { get; set; }
    }
}
