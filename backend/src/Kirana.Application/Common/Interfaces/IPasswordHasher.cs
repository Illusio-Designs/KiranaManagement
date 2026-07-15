namespace Kirana.Application.Common.Interfaces;

/// <summary>Hashes and verifies passwords (implemented in Infrastructure).</summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string hash, string password);
}
