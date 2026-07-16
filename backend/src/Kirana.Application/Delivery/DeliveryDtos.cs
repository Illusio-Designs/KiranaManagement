using Kirana.Domain.Common.Enums;

namespace Kirana.Application.Delivery;

public record PickupPointDto(Guid Id, Guid StoreId, string StoreName, string Address, string? Phone, bool Collected);

public record DeliveryDto(
    Guid Id,
    Guid OrderId,
    string Provider,
    string ExternalTaskId,
    string? TrackingUrl,
    DeliveryStatus Status,
    decimal Fee,
    string? RiderName,
    string? RiderPhone,
    decimal CodAmount,
    bool CodCollected,
    string? PodReference,
    DateTime? DeliveredAt,
    IReadOnlyList<PickupPointDto> Pickups);

/// <summary>Inbound 3PL webhook payload (partner status update).</summary>
public record DeliveryWebhookRequest(
    string ExternalTaskId,
    string Status,
    string? RiderName,
    string? RiderPhone,
    string? PodReference);
