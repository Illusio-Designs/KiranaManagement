using Kirana.Domain.Common;
using Kirana.Domain.Common.Enums;

namespace Kirana.Domain.Purchasing;

/// <summary>
/// A purchase order to a supplier. On receipt (GRN) each line increases the
/// corresponding variant's stock. Tenant-scoped.
/// </summary>
public class PurchaseOrder : BaseEntity, ITenantEntity
{
    public Guid StoreId { get; set; }
    public Guid SupplierId { get; set; }

    public string PoNumber { get; set; } = string.Empty;
    public PurchaseStatus Status { get; set; } = PurchaseStatus.Draft;
    public decimal TotalCost { get; set; }
    public string? Notes { get; set; }
    public DateTime? ReceivedAt { get; set; }

    public ICollection<PurchaseOrderLine> Lines { get; set; } = new List<PurchaseOrderLine>();
}
