using Google.Apis.Auth;
using Kirana.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kirana.Infrastructure.Identity;

/// <summary>
/// Verifies a Google Sign-In ID token using Google's public keys, checking the
/// audience against our configured OAuth client id.
/// </summary>
public class GoogleTokenValidator : IGoogleTokenValidator
{
    private readonly GoogleAuthOptions _options;
    private readonly ILogger<GoogleTokenValidator> _logger;

    public GoogleTokenValidator(IOptions<GoogleAuthOptions> options, ILogger<GoogleTokenValidator> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<GoogleUserInfo?> ValidateAsync(string idToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(idToken))
            return null;

        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings();
            if (!string.IsNullOrWhiteSpace(_options.ClientId))
                settings.Audience = new[] { _options.ClientId };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            var emailVerified = Convert.ToBoolean(payload.EmailVerified);
            return new GoogleUserInfo(payload.Email, payload.Name, emailVerified);
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogWarning(ex, "Google ID token validation failed.");
            return null;
        }
    }
}
