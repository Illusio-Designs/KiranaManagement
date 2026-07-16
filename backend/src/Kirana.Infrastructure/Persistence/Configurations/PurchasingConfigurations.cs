using Kirana.Domain.Purchasing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kirana.Infrastructure.Persistence.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Phone).HasMaxLength(20);
        builder.Property(s => s.Email).HasMaxLength(256);
        builder.Property(s => s.Gstin).HasMaxLength(20);
        builder.Property(s => s.AddressLine).HasMaxLength(500);
        builder.HasIndex(s => s.StoreId);
    }
}

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("purchase_orders");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.PoNumber).IsRequired().HasMaxLength(40);
        builder.Property(p => p.Status).HasConversion<int>();
        builder.Property(p => p.TotalCost).HasColumnType("decimal(18,2)");
        builder.Property(p => p.Notes).HasMaxLength(500);

        builder.HasMany(p => p.Lines)
            .WithOne(l => l.PurchaseOrder!)
            .HasForeignKey(l => l.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.StoreId);
        builder.HasIndex(p => new { p.StoreId, p.PoNumber }).IsUnique();
    }
}

public class PurchaseOrderLineConfiguration : IEntityTypeConfiguration<PurchaseOrderLine>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderLine> builder)
    {
        builder.ToTable("purchase_order_lines");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.VariantName).HasMaxLength(120);
        builder.Property(l => l.UnitCost).HasColumnType("decimal(18,2)");
        builder.Property(l => l.LineTotal).HasColumnType("decimal(18,2)");
        builder.HasIndex(l => l.PurchaseOrderId);
    }
}
