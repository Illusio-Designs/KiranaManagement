using Kirana.Application.Stores.Dtos;

namespace Kirana.Application.Stores;

public interface IStoreService
{
    /// <summary>Registers a store (status = Pending) and its owner account.</summary>
    Task<StoreDto> RegisterAsync(RegisterStoreRequest request, CancellationToken ct = default);

    Task<StoreDto> GetByIdAsync(Guid storeId, CancellationToken ct = default);

    /// <summary>Super Admin: stores awaiting approval.</summary>
    Task<IReadOnlyList<StoreDto>> GetPendingAsync(CancellationToken ct = default);

    Task<IReadOnlyList<StoreDto>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Super Admin: approve a pending store (activates it).</summary>
    Task<StoreDto> ApproveAsync(Guid storeId, CancellationToken ct = default);

    /// <summary>Super Admin: reject a pending store with a reason.</summary>
    Task<StoreDto> RejectAsync(Guid storeId, string reason, CancellationToken ct = default);
}
