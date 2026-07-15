using Kirana.Domain.Common;

namespace Kirana.Domain.Geo;

/// <summary>Platform-level reference data (not tenant-scoped). Drives the
/// country → state → city cascade on store registration.</summary>
public class Country : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Iso2 { get; set; } = string.Empty;   // e.g. "IN"
    public string PhoneCode { get; set; } = string.Empty; // e.g. "+91"

    public ICollection<State> States { get; set; } = new List<State>();
}
