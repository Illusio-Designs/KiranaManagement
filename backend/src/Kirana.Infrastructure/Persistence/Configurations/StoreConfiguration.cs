using Kirana.Domain.Platform;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kirana.Infrastructure.Persistence.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("stores");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.OwnerName).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Email).IsRequired().HasMaxLength(256);
        builder.Property(s => s.Phone).IsRequired().HasMaxLength(20);
        builder.Property(s => s.AddressLine).HasMaxLength(500);
        builder.Property(s => s.City).HasMaxLength(100);
        builder.Property(s => s.Pincode).HasMaxLength(12);
        builder.Property(s => s.Gstin).HasMaxLength(20);
        builder.Property(s => s.RejectionReason).HasMaxLength(500);
        builder.Property(s => s.Status).HasConversion<int>();

        builder.HasIndex(s => s.Email);
        builder.HasIndex(s => s.Status);
    }
}
