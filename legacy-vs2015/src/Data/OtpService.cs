using System;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Configuration;
using Kirana.WebApi.Models;

namespace Kirana.WebApi.Data
{
    // Issues and verifies one-time passcodes for consumer phone login.
    // No SMS gateway is wired for the course build: in dev mode the code is
    // returned in the API response so it can be tested end-to-end. Flip
    // appSetting "OtpDevMode" to false and plug in a real sender for production.
    public static class OtpService
    {
        private const int ValidMinutes = 5;
        private const int MaxAttempts = 5;

        public static bool DevMode
        {
            get
            {
                var v = WebConfigurationManager.AppSettings["OtpDevMode"];
                // default: dev mode ON (course/local build)
                return string.IsNullOrWhiteSpace(v) || v.Trim().ToLowerInvariant() == "true";
            }
        }

        // Creates a fresh code for the phone, invalidating older un-consumed ones.
        public static string Issue(KiranaDbContext db, string phone)
        {
            phone = Normalize(phone);

            var stale = db.OtpCodes.Where(o => o.Phone == phone && !o.Consumed).ToList();
            foreach (var s in stale) s.Consumed = true;

            var code = GenerateCode();
            db.OtpCodes.Add(new OtpCode
            {
                Phone = phone,
                Code = code,
                ExpiresAt = DateTime.UtcNow.AddMinutes(ValidMinutes)
            });
            db.SaveChanges();

            // TODO(prod): send `code` via SMS provider here.
            return code;
        }

        // Returns true if the code is valid for the phone (and consumes it).
        public static bool Verify(KiranaDbContext db, string phone, string code)
        {
            phone = Normalize(phone);
            if (string.IsNullOrWhiteSpace(code)) return false;

            var otp = db.OtpCodes
                .Where(o => o.Phone == phone && !o.Consumed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault();

            if (otp == null) return false;
            if (otp.ExpiresAt < DateTime.UtcNow) return false;
            if (otp.Attempts >= MaxAttempts) return false;

            otp.Attempts++;
            if (otp.Code != code.Trim())
            {
                db.SaveChanges();
                return false;
            }

            otp.Consumed = true;
            db.SaveChanges();
            return true;
        }

        public static string Normalize(string phone)
        {
            return (phone ?? "").Trim();
        }

        private static string GenerateCode()
        {
            // cryptographically-random 6-digit code
            var bytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(bytes);
            var n = BitConverter.ToUInt32(bytes, 0) % 1000000u;
            return n.ToString("D6");
        }
    }
}
