using Kirana.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kirana.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Sku).HasMaxLength(60);
        builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
        builder.Property(p => p.TaxRate).HasColumnType("decimal(5,2)");

        builder.HasIndex(p => p.StoreId);
        builder.HasIndex(p => new { p.StoreId, p.Sku });
    }
}
