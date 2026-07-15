using System.Security.Cryptography;
using Kirana.Application.Common.Exceptions;
using Kirana.Application.Common.Interfaces;
using Kirana.Domain.Customers;
using Microsoft.EntityFrameworkCore;

namespace Kirana.Application.Customers;

public class CustomerAuthService : ICustomerAuthService
{
    private const int OtpTtlSeconds = 300;   // 5 minutes
    private const int MaxAttempts = 5;

    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IOtpSender _otpSender;
    private readonly IJwtTokenService _jwt;

    public CustomerAuthService(IAppDbContext db, IPasswordHasher hasher, IOtpSender otpSender, IJwtTokenService jwt)
    {
        _db = db;
        _hasher = hasher;
        _otpSender = otpSender;
        _jwt = jwt;
    }

    public async Task<RequestOtpResponse> RequestOtpAsync(RequestOtpRequest request, CancellationToken ct = default)
    {
        var phone = NormalizePhone(request.Phone);

        var code = GenerateCode();
        _db.OtpCodes.Add(new OtpCode
        {
            Phone = phone,
            CodeHash = _hasher.Hash(code),
            ExpiresAt = DateTime.UtcNow.AddSeconds(OtpTtlSeconds),
            IsConsumed = false,
            Attempts = 0
        });
        await _db.SaveChangesAsync(ct);

        await _otpSender.SendAsync(phone, code, ct);

        return new RequestOtpResponse(phone, OtpTtlSeconds, "An OTP has been sent to your phone.");
    }

    public async Task<CustomerAuthResponse> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken ct = default)
    {
        var phone = NormalizePhone(request.Phone);

        var otp = await _db.OtpCodes
            .Where(o => o.Phone == phone && !o.IsConsumed)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (otp is null || otp.ExpiresAt < DateTime.UtcNow)
            throw AppException.Unauthorized("OTP is invalid or has expired. Please request a new one.");

        if (otp.Attempts >= MaxAttempts)
            throw AppException.Unauthorized("Too many attempts. Please request a new OTP.");

        otp.Attempts++;

        if (!_hasher.Verify(otp.CodeHash, request.Code.Trim()))
        {
            await _db.SaveChangesAsync(ct);
            throw AppException.Unauthorized("Incorrect OTP.");
        }

        otp.IsConsumed = true;

        var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Phone == phone, ct);
        if (customer is null)
        {
            customer = new Customer { Phone = phone, IsVerified = true };
            _db.Customers.Add(customer);
        }
        customer.IsVerified = true;
        customer.LastLoginAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        var (token, expiresAt) = _jwt.CreateCustomerToken(customer);
        return new CustomerAuthResponse(token, expiresAt, customer.Id, customer.Phone, customer.FullName);
    }

    private static string NormalizePhone(string phone)
    {
        var trimmed = phone.Trim();
        if (string.IsNullOrEmpty(trimmed))
            throw new AppException("Phone number is required.");
        return trimmed;
    }

    private static string GenerateCode()
    {
        // 6-digit numeric OTP.
        return RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
    }
}
