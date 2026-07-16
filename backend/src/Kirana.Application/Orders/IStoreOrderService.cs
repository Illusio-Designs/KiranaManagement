using Kirana.Domain.Common.Enums;

namespace Kirana.Application.Orders;

/// <summary>Store-side management of that store's parts of marketplace orders.</summary>
public interface IStoreOrderService
{
    Task<IReadOnlyList<StoreOrderDto>> GetStoreOrdersAsync(StoreOrderStatus? status, CancellationToken ct = default);
    Task<StoreOrderDto> GetStoreOrderAsync(Guid storeOrderId, CancellationToken ct = default);

    Task<StoreOrderDto> AcceptAsync(Guid storeOrderId, CancellationToken ct = default);
    Task<StoreOrderDto> PackAsync(Guid storeOrderId, CancellationToken ct = default);
    Task<StoreOrderDto> MarkReadyAsync(Guid storeOrderId, CancellationToken ct = default);
    Task<StoreOrderDto> CancelAsync(Guid storeOrderId, CancellationToken ct = default);
}
