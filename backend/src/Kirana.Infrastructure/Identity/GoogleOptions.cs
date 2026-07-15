namespace Kirana.Infrastructure.Identity;

public class GoogleAuthOptions
{
    public const string SectionName = "Google";

    /// <summary>OAuth 2.0 Web client ID; the ID token's audience must match this.</summary>
    public string ClientId { get; set; } = string.Empty;
}
