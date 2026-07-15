using Kirana.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Kirana.Infrastructure.Integrations.Otp;

/// <summary>
/// Development OTP "sender" — logs the code instead of sending an SMS, so the
/// OTP flow is testable without an SMS provider. Replace with a real provider
/// (e.g. MSG91) in production.
/// </summary>
public class DevOtpSender : IOtpSender
{
    private readonly ILogger<DevOtpSender> _logger;

    public DevOtpSender(ILogger<DevOtpSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string phone, string code, CancellationToken ct = default)
    {
        _logger.LogWarning("DEV OTP for {Phone}: {Code} (no SMS sent — dev sender)", phone, code);
        return Task.CompletedTask;
    }
}
