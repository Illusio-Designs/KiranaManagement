using System;
using System.Security.Cryptography;

namespace Kirana.WebApi.Data
{
    // PBKDF2 password hashing (no external packages). Layout: 16-byte salt +
    // 32-byte hash, base64-encoded. Uses the 3-arg Rfc2898DeriveBytes ctor,
    // which is available on .NET Framework 4.6 (defaults to SHA1).
    public static class PasswordHasher
    {
        private const int Iterations = 10000;

        public static string Hash(string password)
        {
            var salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(salt);

            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
                hash = pbkdf2.GetBytes(32);

            var combined = new byte[48];
            Buffer.BlockCopy(salt, 0, combined, 0, 16);
            Buffer.BlockCopy(hash, 0, combined, 16, 32);
            return Convert.ToBase64String(combined);
        }

        public static bool Verify(string stored, string password)
        {
            try
            {
                var combined = Convert.FromBase64String(stored);
                if (combined.Length != 48) return false;

                var salt = new byte[16];
                Buffer.BlockCopy(combined, 0, salt, 0, 16);
                var hash = new byte[32];
                Buffer.BlockCopy(combined, 16, hash, 0, 32);

                byte[] test;
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
                    test = pbkdf2.GetBytes(32);

                for (var i = 0; i < 32; i++)
                    if (hash[i] != test[i]) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
