namespace Kirana.Application.Delivery;

public interface IDeliveryService
{
    /// <summary>Books a 3PL delivery task for a ready order (multi-store pickup + single drop).</summary>
    Task<DeliveryDto> BookAsync(Guid orderId, CancellationToken ct = default);

    Task<DeliveryDto> GetForOrderAsync(Guid orderId, CancellationToken ct = default);

    /// <summary>Processes a partner status update (webhook) and syncs the order.</summary>
    Task HandleWebhookAsync(string provider, DeliveryWebhookRequest payload, CancellationToken ct = default);
}
