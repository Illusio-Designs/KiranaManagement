using Kirana.Domain.Delivery;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kirana.Infrastructure.Persistence.Configurations;

public class DeliveryTaskConfiguration : IEntityTypeConfiguration<DeliveryTask>
{
    public void Configure(EntityTypeBuilder<DeliveryTask> builder)
    {
        builder.ToTable("delivery_tasks");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Provider).IsRequired().HasMaxLength(60);
        builder.Property(t => t.ExternalTaskId).IsRequired().HasMaxLength(120);
        builder.Property(t => t.TrackingUrl).HasMaxLength(500);
        builder.Property(t => t.Status).HasConversion<int>();
        builder.Property(t => t.Fee).HasColumnType("decimal(18,2)");
        builder.Property(t => t.CodAmount).HasColumnType("decimal(18,2)");
        builder.Property(t => t.RiderName).HasMaxLength(200);
        builder.Property(t => t.RiderPhone).HasMaxLength(20);
        builder.Property(t => t.PodReference).HasMaxLength(200);

        builder.HasMany(t => t.Pickups)
            .WithOne(p => p.DeliveryTask!)
            .HasForeignKey(p => p.DeliveryTaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.OrderId).IsUnique();
        builder.HasIndex(t => new { t.Provider, t.ExternalTaskId });
    }
}

public class PickupPointConfiguration : IEntityTypeConfiguration<PickupPoint>
{
    public void Configure(EntityTypeBuilder<PickupPoint> builder)
    {
        builder.ToTable("pickup_points");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.StoreName).HasMaxLength(200);
        builder.Property(p => p.Address).HasMaxLength(600);
        builder.Property(p => p.Phone).HasMaxLength(20);
        builder.HasIndex(p => p.DeliveryTaskId);
    }
}
