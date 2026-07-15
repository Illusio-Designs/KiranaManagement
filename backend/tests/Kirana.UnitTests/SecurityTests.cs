using Kirana.Domain.Common.Enums;
using Kirana.Domain.Identity;
using Kirana.Infrastructure.Identity;
using Microsoft.Extensions.Options;
using Xunit;

namespace Kirana.UnitTests;

public class SecurityTests
{
    [Fact]
    public void PasswordHasher_RoundTrips()
    {
        var hasher = new PasswordHasherAdapter();
        var hash = hasher.Hash("S3cret!");

        Assert.NotEqual("S3cret!", hash);
        Assert.True(hasher.Verify(hash, "S3cret!"));
        Assert.False(hasher.Verify(hash, "wrong"));
    }

    [Fact]
    public void JwtToken_ContainsStoreClaim_ForStoreUser()
    {
        var options = Options.Create(new JwtOptions
        {
            Issuer = "test",
            Audience = "test",
            Key = "unit-test-signing-key-at-least-32-bytes-long-000000",
            ExpiryMinutes = 60
        });
        var service = new JwtTokenService(options);

        var storeId = Guid.NewGuid();
        var user = new User
        {
            Email = "owner@store.local",
            FullName = "Owner",
            Role = UserRole.Owner,
            StoreId = storeId
        };

        var (token, expiresAt) = service.CreateToken(user);

        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.True(expiresAt > DateTime.UtcNow);
        Assert.Contains(storeId.ToString(), DecodePayload(token));
    }

    // Decodes the JWT payload segment (base64url) to plain JSON for assertions.
    private static string DecodePayload(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var padded = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=')
            .Replace('-', '+').Replace('_', '/');
        return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(padded));
    }
}
