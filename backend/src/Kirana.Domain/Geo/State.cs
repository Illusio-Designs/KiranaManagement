using Kirana.Domain.Common;

namespace Kirana.Domain.Geo;

public class State : BaseEntity
{
    public Guid CountryId { get; set; }
    public Country? Country { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }   // e.g. "MH"

    public ICollection<City> Cities { get; set; } = new List<City>();
}
