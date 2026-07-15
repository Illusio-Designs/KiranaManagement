namespace Kirana.Application.Common.Interfaces;

/// <summary>Delivers a one-time code to a phone (SMS/WhatsApp). Dev impl logs it;
/// production wires an SMS provider (e.g. MSG91) — see docs/COSTS.md.</summary>
public interface IOtpSender
{
    Task SendAsync(string phone, string code, CancellationToken ct = default);
}
