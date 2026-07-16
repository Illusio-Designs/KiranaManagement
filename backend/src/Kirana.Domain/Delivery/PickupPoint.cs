using Kirana.Domain.Common;

namespace Kirana.Domain.Delivery;

/// <summary>One store the rider must collect from for a delivery task.</summary>
public class PickupPoint : BaseEntity
{
    public Guid DeliveryTaskId { get; set; }
    public DeliveryTask? DeliveryTask { get; set; }

    public Guid StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Phone { get; set; }

    public bool Collected { get; set; }
}
