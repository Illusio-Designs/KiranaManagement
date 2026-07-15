namespace Kirana.Application.Customers;

public interface ICustomerAuthService
{
    Task<RequestOtpResponse> RequestOtpAsync(RequestOtpRequest request, CancellationToken ct = default);
    Task<CustomerAuthResponse> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken ct = default);
}
