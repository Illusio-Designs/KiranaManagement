using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Kirana.Application.Common.Interfaces;
using Kirana.Domain.Customers;
using Kirana.Domain.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Kirana.Infrastructure.Identity;

public class JwtTokenService : IJwtTokenService
{
    public const string StoreIdClaim = "store_id";
    public const string CustomerRole = "Customer";

    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public (string Token, DateTime ExpiresAt) CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        if (user.StoreId is Guid storeId)
            claims.Add(new Claim(StoreIdClaim, storeId.ToString()));

        return Build(claims);
    }

    public (string Token, DateTime ExpiresAt) CreateCustomerToken(Customer customer)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, customer.FullName ?? customer.Phone),
            new(ClaimTypes.Role, CustomerRole),
            new("phone", customer.Phone)
        };

        if (!string.IsNullOrEmpty(customer.Email))
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, customer.Email));

        return Build(claims);
    }

    private (string Token, DateTime ExpiresAt) Build(IEnumerable<Claim> claims)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
