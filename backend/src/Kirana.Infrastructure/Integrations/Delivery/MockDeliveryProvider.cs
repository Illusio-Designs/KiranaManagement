using Kirana.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Kirana.Infrastructure.Integrations.Delivery;

/// <summary>
/// Development 3PL provider — simulates booking/tracking without calling a real
/// courier API, so the delivery flow is testable end-to-end. Replace with real
/// providers (Porter/Borzo/Shiprocket) behind <see cref="IDeliveryProvider"/>.
/// </summary>
public class MockDeliveryProvider : IDeliveryProvider
{
    private readonly ILogger<MockDeliveryProvider> _logger;

    public MockDeliveryProvider(ILogger<MockDeliveryProvider> logger)
    {
        _logger = logger;
    }

    public string Name => "Mock";

    public Task<DeliveryQuote> GetQuoteAsync(DeliveryRequest request, CancellationToken ct = default)
        => Task.FromResult(new DeliveryQuote(Fee(request), 30, Serviceable: true));

    public Task<DeliveryTaskResult> CreateTaskAsync(DeliveryRequest request, CancellationToken ct = default)
    {
        var externalId = Guid.NewGuid().ToString("N");
        _logger.LogInformation("Mock 3PL: booked task {ExternalId} for order {OrderNumber} ({Pickups} pickups).",
            externalId, request.OrderNumber, request.Pickups.Count);
        return Task.FromResult(new DeliveryTaskResult(
            externalId, $"https://track.mock-delivery.local/{externalId}", Fee(request)));
    }

    public Task<DeliveryTrackingInfo> TrackAsync(string externalTaskId, CancellationToken ct = default)
        => Task.FromResult(new DeliveryTrackingInfo("RiderAssigned", "Mock Rider", "+910000000000",
            $"https://track.mock-delivery.local/{externalTaskId}"));

    // Flat base + per-extra-pickup, mirroring the hyperlocal model in docs/COSTS.md.
    private static decimal Fee(DeliveryRequest request)
        => 40m + 20m * Math.Max(0, request.Pickups.Count - 1);
}
