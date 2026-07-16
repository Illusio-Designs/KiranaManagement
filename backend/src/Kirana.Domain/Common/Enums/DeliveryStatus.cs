namespace Kirana.Domain.Common.Enums;

/// <summary>Status of a delivery task booked with a 3PL partner (PRD §5.8).</summary>
public enum DeliveryStatus
{
    Searching = 0,       // looking for a rider
    RiderAssigned = 1,
    AtStore = 2,
    PickedUp = 3,
    OutForDelivery = 4,
    Delivered = 5,
    Failed = 6,
    Cancelled = 7
}
