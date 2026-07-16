namespace Kirana.Application.Common.Interfaces;

public record GeoStop(string Name, string Address, string? Phone);

public record DeliveryRequest(
    string OrderNumber,
    IReadOnlyList<GeoStop> Pickups,
    GeoStop Drop,
    decimal CodAmount);

public record DeliveryQuote(decimal Fee, int EtaMinutes, bool Serviceable);
public record DeliveryTaskResult(string ExternalTaskId, string TrackingUrl, decimal Fee);
public record DeliveryTrackingInfo(string Status, string? RiderName, string? RiderPhone, string? TrackingUrl);

/// <summary>
/// A third-party logistics partner (Porter/Borzo/Shiprocket…). All partners sit
/// behind this seam so they can be added/switched or failed over
/// (see docs/DELIVERY_INTEGRATIONS.md).
/// </summary>
public interface IDeliveryProvider
{
    string Name { get; }
    Task<DeliveryQuote> GetQuoteAsync(DeliveryRequest request, CancellationToken ct = default);
    Task<DeliveryTaskResult> CreateTaskAsync(DeliveryRequest request, CancellationToken ct = default);
    Task<DeliveryTrackingInfo> TrackAsync(string externalTaskId, CancellationToken ct = default);
}
