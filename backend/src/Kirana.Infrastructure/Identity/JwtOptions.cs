namespace Kirana.Infrastructure.Identity;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "KiranaManagement";
    public string Audience { get; set; } = "KiranaManagement";
    public string Key { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 480; // 8 hours
}
