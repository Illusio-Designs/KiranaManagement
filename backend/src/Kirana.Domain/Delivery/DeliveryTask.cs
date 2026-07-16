using Kirana.Domain.Common;
using Kirana.Domain.Common.Enums;

namespace Kirana.Domain.Delivery;

/// <summary>
/// A delivery booked with a third-party logistics partner for a marketplace
/// order. Platform-level (spans stores): one rider collects from each store
/// (a <see cref="PickupPoint"/>) and delivers together (PRD §5.8).
/// </summary>
public class DeliveryTask : BaseEntity
{
    public Guid OrderId { get; set; }

    public string Provider { get; set; } = string.Empty;
    public string ExternalTaskId { get; set; } = string.Empty;
    public string? TrackingUrl { get; set; }

    public DeliveryStatus Status { get; set; } = DeliveryStatus.Searching;

    public decimal Fee { get; set; }

    public string? RiderName { get; set; }
    public string? RiderPhone { get; set; }

    // COD + proof of delivery
    public decimal CodAmount { get; set; }
    public bool CodCollected { get; set; }
    public string? PodReference { get; set; }   // OTP / photo id / signature ref
    public DateTime? DeliveredAt { get; set; }

    public ICollection<PickupPoint> Pickups { get; set; } = new List<PickupPoint>();
}
