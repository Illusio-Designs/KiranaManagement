using Kirana.Application.Common.Interfaces;
using Kirana.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace Kirana.Infrastructure.Identity;

/// <summary>
/// Wraps ASP.NET Core's <see cref="PasswordHasher{TUser}"/> (PBKDF2) behind our
/// own <see cref="IPasswordHasher"/> so the Application layer stays framework-free.
/// The user argument is unused by the default algorithm, so a shared instance is fine.
/// </summary>
public class PasswordHasherAdapter : IPasswordHasher
{
    private static readonly User Dummy = new();
    private readonly PasswordHasher<User> _inner = new();

    public string Hash(string password) => _inner.HashPassword(Dummy, password);

    public bool Verify(string hash, string password)
    {
        var result = _inner.VerifyHashedPassword(Dummy, hash, password);
        return result is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
