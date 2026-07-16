using Kirana.Domain.Common.Enums;

namespace Kirana.Application.Orders;

public record CheckoutRequest(
    string ContactName,
    string ContactPhone,
    string AddressLine,
    string? City,
    string? Pincode,
    PaymentMode PaymentMode);

public interface ICheckoutService
{
    /// <summary>Converts the customer's cart into a parent order split per store.</summary>
    Task<OrderDto> CheckoutAsync(Guid customerId, CheckoutRequest request, CancellationToken ct = default);

    Task<IReadOnlyList<OrderDto>> GetMyOrdersAsync(Guid customerId, CancellationToken ct = default);
    Task<OrderDto> GetMyOrderAsync(Guid customerId, Guid orderId, CancellationToken ct = default);
}
