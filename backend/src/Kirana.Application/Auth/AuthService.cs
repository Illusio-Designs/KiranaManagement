using Kirana.Application.Auth.Dtos;
using Kirana.Application.Common.Exceptions;
using Kirana.Application.Common.Interfaces;
using Kirana.Domain.Common.Enums;
using Kirana.Domain.Platform;
using Microsoft.EntityFrameworkCore;

namespace Kirana.Application.Auth;

public class AuthService : IAuthService
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwt;

    public AuthService(IAppDbContext db, IPasswordHasher passwordHasher, IJwtTokenService jwt)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwt = jwt;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        if (user is null || !_passwordHasher.Verify(user.PasswordHash, request.Password))
            throw AppException.Unauthorized("Invalid email or password.");

        if (!user.IsActive)
            throw AppException.Unauthorized("This account is disabled.");

        // Store staff can only log in once their store is Active (PRD FR-1.3).
        if (user.Role != UserRole.SuperAdmin && user.StoreId is Guid sid)
        {
            var store = await _db.Stores.FirstOrDefaultAsync(s => s.Id == sid, ct);
            if (store is null)
                throw AppException.Unauthorized("Store not found for this account.");
            if (store.Status != StoreStatus.Active)
                throw AppException.Unauthorized($"Your store is not active yet (status: {store.Status}).");
        }

        var (token, expiresAt) = _jwt.CreateToken(user);
        return new AuthResponse(token, expiresAt, user.Id, user.Email, user.FullName, user.Role, user.StoreId);
    }
}
