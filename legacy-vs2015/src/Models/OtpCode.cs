using System;
using System.ComponentModel.DataAnnotations;

namespace Kirana.WebApi.Models
{
    // A one-time passcode issued for consumer phone login.
    // The latest un-consumed row for a phone is the active challenge.
    public class OtpCode
    {
        public OtpCode()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(20)]
        public string Phone { get; set; }

        [Required, MaxLength(6)]
        public string Code { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool Consumed { get; set; }
        public int Attempts { get; set; }
    }
}
