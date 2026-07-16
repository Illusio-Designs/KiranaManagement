using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kirana.WebApi.Models
{
    // A guest marketplace order (consumer checkout). Can span multiple stores.
    public class Order
    {
        public Order()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Lines = new List<OrderLine>();
        }

        [Key]
        public Guid Id { get; set; }

        public Guid? CustomerId { get; set; }

        [MaxLength(40)]
        public string OrderNumber { get; set; }

        [MaxLength(200)]
        public string CustomerName { get; set; }
        [MaxLength(20)]
        public string Phone { get; set; }
        [MaxLength(500)]
        public string Address { get; set; }

        // Delivery area — shown to stores instead of the full address so that
        // consumer personal data is not exposed in the store order view.
        [MaxLength(100)]
        public string City { get; set; }
        [MaxLength(12)]
        public string Pincode { get; set; }

        // Delivery geo-location captured at checkout; used to estimate the
        // distance/ETA from the fulfilling store.
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? DistanceKm { get; set; }
        public int? EtaMinutes { get; set; }

        public OrderStatus Status { get; set; }

        public decimal Subtotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal GrandTotal { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<OrderLine> Lines { get; set; }
    }

    public class OrderLine
    {
        public OrderLine()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }

        public Guid StoreId { get; set; }
        [MaxLength(200)]
        public string StoreName { get; set; }

        public Guid ProductVariantId { get; set; }
        [MaxLength(200)]
        public string ProductName { get; set; }
        [MaxLength(120)]
        public string VariantName { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
