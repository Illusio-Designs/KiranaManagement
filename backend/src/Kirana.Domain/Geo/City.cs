using Kirana.Domain.Common;

namespace Kirana.Domain.Geo;

public class City : BaseEntity
{
    public Guid StateId { get; set; }
    public State? State { get; set; }

    public string Name { get; set; } = string.Empty;
}
