using Kirana.Domain.Customers;
using Kirana.Domain.Platform;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kirana.Infrastructure.Persistence.Configurations;

public class StoreDocumentConfiguration : IEntityTypeConfiguration<StoreDocument>
{
    public void Configure(EntityTypeBuilder<StoreDocument> builder)
    {
        builder.ToTable("store_documents");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.FileName).IsRequired().HasMaxLength(260);
        builder.Property(d => d.ContentType).IsRequired().HasMaxLength(120);
        builder.Property(d => d.StoragePath).IsRequired().HasMaxLength(500);
        builder.Property(d => d.DocumentType).HasConversion<int>();
        builder.HasIndex(d => d.StoreId);
    }
}

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Phone).IsRequired().HasMaxLength(20);
        builder.Property(c => c.Email).HasMaxLength(256);
        builder.Property(c => c.FullName).HasMaxLength(200);
        builder.HasIndex(c => c.Phone).IsUnique();
    }
}

public class OtpCodeConfiguration : IEntityTypeConfiguration<OtpCode>
{
    public void Configure(EntityTypeBuilder<OtpCode> builder)
    {
        builder.ToTable("otp_codes");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Phone).IsRequired().HasMaxLength(20);
        builder.Property(o => o.CodeHash).IsRequired().HasMaxLength(500);
        builder.HasIndex(o => o.Phone);
    }
}
