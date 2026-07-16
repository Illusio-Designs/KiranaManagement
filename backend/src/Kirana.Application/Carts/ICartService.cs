namespace Kirana.Application.Carts;

public interface ICartService
{
    Task<CartSummaryDto> GetSummaryAsync(Guid customerId, CancellationToken ct = default);
    Task<CartSummaryDto> AddItemAsync(Guid customerId, AddCartItemRequest request, CancellationToken ct = default);
    Task<CartSummaryDto> UpdateItemAsync(Guid customerId, Guid itemId, UpdateCartItemRequest request, CancellationToken ct = default);
    Task<CartSummaryDto> RemoveItemAsync(Guid customerId, Guid itemId, CancellationToken ct = default);
    Task<CartSummaryDto> ClearAsync(Guid customerId, CancellationToken ct = default);
}
