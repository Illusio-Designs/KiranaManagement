namespace Kirana.Domain.Common;

/// <summary>
/// Base for every persisted entity: GUID id + audit timestamps.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
